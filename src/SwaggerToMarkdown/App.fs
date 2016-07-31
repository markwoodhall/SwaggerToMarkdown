module App

open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful
open Suave.Web
open FSharp.Data

type Swagger =
    JsonProvider<"./swagger.json">

let getSwagger swaggerUrl =
    match swaggerUrl with
    | Choice1Of2 s -> Swagger.Load (s:string) |> Some
    | Choice2Of2 s -> None

let getOverview (swagger:Swagger.Root) =
    sprintf "#%s\n%s" swagger.Info.Title swagger.Info.Description

let getPaths (paths:Swagger.Paths) =
    paths.JsonValue.Properties()
    |> Seq.map (fun (k, _) -> "#" + k)
    |> Seq.reduce (fun acc elem -> acc + "\n" + elem)

let getMarkdown (swagger:Swagger.Root) =
    let overview = getOverview swagger
    let paths = getPaths swagger.Paths
    sprintf "%s\n%s" overview paths

let markdown : WebPart =
    path "/markdown" >=> choose [
      GET  >=> request(fun r ->
          let swaggerUrl = r.queryParam("swaggerUrl")
          let swagger = getSwagger swaggerUrl
          OK <| match swagger with
                | Some s -> getMarkdown s
                | None -> "Could not parse swaggerUrl parameter"
      )
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
