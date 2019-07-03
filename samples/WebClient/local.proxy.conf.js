const PROXY_CONFIG = [
  {
    context: [
      "/connect",
      "/api"
    ],
    target: "https://localhost:5028",
    secure: false
  }
]

module.exports = PROXY_CONFIG;
