const PROXY_CONFIG = [
  {
    context: [
      "/connect",
      "/api"
    ],
    target: "https://localhost:5001",
    secure: false
  }
]

module.exports = PROXY_CONFIG;
