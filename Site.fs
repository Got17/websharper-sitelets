namespace Sitelets

open WebSharper
open WebSharper.Sitelets
open WebSharper.UI
open WebSharper.UI.Server

type EndPoint =
    | [<EndPoint "/">] Home
    | [<EndPoint "/about">] About

module Templating =
    open WebSharper.UI.Html

    // Compute a menubar where the menu item for the given endpoint is active
    let MenuBar (ctx: Context<EndPoint>) endpoint : Doc list =
        let ( => ) txt act =
            let isActive = if endpoint = act then "nav-link active" else "nav-link"
            li [attr.``class`` "nav-item"] [
                a [
                    attr.``class`` isActive
                    attr.href (ctx.Link act)
                ] [text txt]
            ]
        [
            "Home" => EndPoint.Home
            "About" => EndPoint.About
        ]

    let Main ctx action (title: string) (body: Doc list) =
        Templates.MainTemplate()
            .Title(title)
            .MenuBar(MenuBar ctx action)
            .Body(body)
            .Doc()

module Site =
    open WebSharper.UI.Html

    open type WebSharper.UI.ClientServer

    let HomePage ctx =
        Content.Page(
            Templating.Main ctx EndPoint.Home "Home" [
                h1 [] [text "Say Hi to the server!"]
                div [] [client (Client.Main())]
            ], 
            Bundle = "home"
        )

    let AboutPage ctx =
        Content.Page(
            Templating.Main ctx EndPoint.About "About" [
                h1 [] [text "About"]
                p [] [text "This is a template WebSharper client-server application."]
            ], 
            Bundle = "about"
        )

    [<Website>]
    let Main =
        Application.MultiPage (fun ctx endpoint ->
            match endpoint with
            | EndPoint.Home -> HomePage ctx
            | EndPoint.About -> AboutPage ctx
        )

