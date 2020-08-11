const PROXY_CONFIG = [
  {
    context: [
      "/connect",
      "/api"
    ],
    target: "http://localhost:5001",
    secure: false
  }
]

module.exports = PROXY_CONFIG;
