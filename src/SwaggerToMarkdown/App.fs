module App

open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful
open Suave.Web
open FSharp.Data

let generateMarkdown swaggerUrl =
    match swaggerUrl with
    | Choice1Of2 s -> sprintf "%s" s
    | Choice2Of2 s -> "Nothing"

let markdown : WebPart =
    path "/markdown" >=> choose [
      GET  >=> request(fun r -> OK <| generateMarkdown (r.queryParam("swaggerUrl")))
      RequestErrors.NOT_FOUND "Unsupported HTTP Method, did you mean to make a GET request" ]

let app =
    choose [
      markdown
      RequestErrors.NOT_FOUND "Resource not found" ]

[<EntryPoint>]
let main argv =
    printfn "%A" argv
    startWebServer defaultConfig app
    0

// curl -X GET http://localhost:8083/markdown?swaggerUrl=http%3A%2F%2Fpetstore.swagger.io%2Fv2%2Fswagger.json
