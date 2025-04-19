window.onload = () => {
  window.ui = SwaggerUIBundle({
    url: "swagger.json", // 👈 Aqui é o arquivo local
    dom_id: "#swagger-ui",
    deepLinking: true,
    presets: [SwaggerUIBundle.presets.apis, SwaggerUIStandalonePreset],
    layout: "StandaloneLayout",
    supportedSubmitMethods: [],
  });
};
