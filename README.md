![NuGet](https://img.shields.io/nuget/v/AspNetCore.Mvc.ViewComponentSlots.svg?style=for-the-badge)
![NuGet](https://img.shields.io/nuget/dt/AspNetCore.Mvc.ViewComponentSlots.svg?style=for-the-badge)

# Summary

This repo provides an experimental implementation for the View Component Slots proposal detailed [here](https://github.com/aspnet/Mvc/issues/8168)

View Component Slots provides a mechanism to render user supplied child content of a View Component tag helper into the View Component's view template.

# Installation

Install the nuget package https://www.nuget.org/packages/AspNetCore.Mvc.ViewComponentSlots/

Place an add tag helpers direction in your `_ViewImports.cshtml`

```html
@addTagHelper *, AspNetCore.Mvc.ViewComponentSlots
```

# View Component Tag Helper

Because we cannot hook into or extend the default AspNetCore.Mvc [View Component Tag Helpers](https://docs.microsoft.com/en-us/aspnet/core/mvc/views/view-components?view=aspnetcore-2.1#invoking-a-view-component-as-a-tag-helper) this project had to implement a custom View Component tag helper that can accept child content. An example of invoking our view component tag helper:

```html
<!-- create a element named after your custom view component in kebab-case with a empty attribute 'vc' -->
<my-custom-component vc>
    <p>SOME CHILD CONTENT!</p>
</my-custom-component>

<!-- optionally you can create an element with PascalCase -->
<MyCustomComponent vc>
    <p>SOME CHILD CONTENT!</p>
</MyCustomComponent>

<!-- or even qualify namespaces -->
<Namespace.To.MyCustomComponent vc>
    <p>SOME CHILD CONTENT!</p>
</Namespace.To.MyCustomComponent>
```

requirements:

* Your view component markup `MUST` include an empty attribute `vc` for the tag helper to work
* Your view component `MUST` inherit from `ViewComponent`
* Currently only supports invoke method `InvokeAsync()`

minimal required view component class:

```cs
public class MyCustomComponent : ViewComponent
{
    public Task<IViewComponentResult> InvokeAsync()
    {
        return Task.FromResult<IViewComponentResult>(View());
    }
}
```

# View Component Slots

In the above examples the supplied child content does not get rendered as there is nowhere in View Component's view template to place the child content. Thus we must define a new `<vc:slot></vc:slot>` element in our view template!

with calling view `Index.cshtml`

```html
<my-custom-component vc>
    <p>SOME CHILD CONTENT!</p>
</my-custom-component>
```

with view component view template `MyCustomComponent.cshtml`

```html
<h1>My Custom Component</h1>
<vc:slot><p>please supply some child content</p></vc:slot>
```

The child content supplied in the view components element (`<p>SOME CHILD CONTENT!</p>`) will now replace the `<vc:slot>` element in your rendered view component view template. Your rendered template now effectivly becomes:

```html
<h1>My Custom Component</h1>
<p>SOME CHILD CONTENT!</p>
```

If you do not supply any child content to the view component tag helper the default `<vc:slot>` child content will be rendered:

```html
<h1>My Custom Component</h1>
<p>please supply some child content</p>
```

A plain `<vc:slot></vc:slot>` element is known as a default unnamed slot

# Named View Component Slots

In many cases you may want to slot different child elements into different locations in your component's view template. To do this we can use named slots and slot selectors.

Define a component view template with multiple named slots `MyCustomComponent.cshtml`

```html
<h1>My Custom Component</h1>
<vc:slot name="foo"><p>please supply foo's child content</p></vc:slot>
<vc:slot name="bar"><p>please supply bar's child content</p></vc:slot>
<vc:slot><p>please supply default child content</p></vc:slot>
```

the above view template marks each `vc:slot` with a `name` attribute turning the slot into a `named` slot. We also specify a default unnamed slot as we've done before.

Define a calling view with multiple child elements targeting different slots using slot selector attributes `vc:slot="[slot_name]"`Index.cshtml`

```html
<my-custom-component vc>
    <p vc:slot="bar">bar's child content</p>
    <p>SOME DEFAULT CHILD CONTENT!</p>
    <p vc:slot="foo">foos's child content</p>
    <p>SOME MORE DEFAULT CHILD CONTENT!</p>
</my-custom-component>
```

The above view component element will render its view template as so:

```html
<h1>My Custom Component</h1>
<p>foos's child content</p>
<p>bar's child content</p>
<p>SOME DEFAULT CHILD CONTENT!</p>
<p>SOME MORE DEFAULT CHILD CONTENT!</p>
```

Its important to note that the ordering of child elements that provide a slot selector attribute `vc:slot="[slot_name]"` does not matter. They will be properly placed in the matching
named slot in the component's view template.

The remaining child elements that do not select a specific slot will be placed in the default view component template slot, if one exists.

# Repeating View Component Slots

You can repeat named and unnamed view template slots as many times as you wish and they will all render the same targeted content

`MyCustomComponent.cshtml`

```html
<h1>My Custom Component</h1>
<vc:slot name="foo"><p>please supply foo's child content</p></vc:slot>
<vc:slot name="foo"><p>please supply foo's child content</p></vc:slot>
<vc:slot name="bar"><p>please supply bar's child content</p></vc:slot>
<vc:slot name="bar"><p>please supply bar's child content</p></vc:slot>
<vc:slot name="bar"><p>please supply bar's child content</p></vc:slot>
<vc:slot><p>please supply default child content</p></vc:slot>
<vc:slot><p>please supply default child content</p></vc:slot>
```

`Index.cshtml`

```html
<my-custom-component vc>
    <p vc:slot="bar">bar's child content</p>
    <p vc:slot="foo">foos's child content</p>
    <p>SOME DEFAULT CHILD CONTENT!</p>
</my-custom-component>
```

Renders:

```html
<h1>My Custom Component</h1>
<p>foos's child content</p>
<p>foos's child content</p>
<p>bar's child content</p>
<p>bar's child content</p>
<p>bar's child content</p>
<p>SOME DEFAULT CHILD CONTENT!</p>
<p>SOME DEFAULT CHILD CONTENT!</p>
```

# Nesting View Components

View Component elements fully support nesting, as in the following example:

`MyCustomComponent.cshtml`

```html
<h1>My Custom Component</h1>
<vc:slot name="foo"><p>please supply foo's child content</p></vc:slot>
<div style="border-left:1px solid black;padding-left:20px;">
    <vc:slot name="nested">no nested content supplied</vc:slot>
</div>
```

`Index.cshtml`

```html
<my-custom-component vc>
    <p vc:slot="foo">foos's child content</p>

    <my-custom-component vc vc:slot="nested">
        <p vc:slot="foo">NESTED foos's child content</p>
    </my-custom-component>
</my-custom-component>
```

Renders:

```html
<h1>My Custom Component</h1>
<p>foos's child content</p>
<div style="border-left:1px solid black;padding-left:20px;">
    <my-custom-component>
        <h1>My Custom Component</h1>
        <p>NESTED foos's child content</p>
        <div style="border-left:1px solid black;padding-left:20px;">
            no nested content supplied
        </div>
    </my-custom-component>
</div>
```

# View Component Element Parameters

View Component elements can also take attributes that will be passed to your View Component's `InvokeAsync()` where attributes prefixed with `param:` are present

View Component with paramters

```cs
public class MyCustomComponent : ViewComponent
{
    public Task<IViewComponentResult> InvokeAsync(string foo, float bar)
    {
        return Task.FromResult<IViewComponentResult>(View());
    }
}
```

passing parameters via attributes:

```html
<my-custom-component vc param:foo="@("hello, world")" param:bar="@(1.3f)">
</my-custom-component>
```