/** @type {import("@rtk-query/codegen-openapi").ConfigFile} */
const config = {
  schemaFile: "http://localhost:5033/swagger/v1/swagger.json",
  apiFile: "./src/store/empty-api.ts",
  apiImport: "emptySplitApi",
  outputFile: "./src/store/api.ts",
  exportName: "api",
  hooks: true,
}

module.exports = config
