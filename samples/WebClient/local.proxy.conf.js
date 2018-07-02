const PROXY_CONFIG = [
  {
      context: [
          "/connect",
          "/api"
      ],
      target: "http://localhost:5000",
      secure: false
  }
]

module.exports = PROXY_CONFIG;