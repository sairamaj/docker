var express = require('express')
var app = express()

/*
  gets all the urls 
    auth
    token
    user
*/
app.get('/', (req, res) => {
  var urls = {
    auth: "http://localhost:5001/auth",
    token: "http://localhost:5001/token",
    user: "http://localhost:5001/user"
  }
  res.status(200).json(urls)
});


/*
  auth url
  generates a code and redirects to the source. 
*/
app.get('/auth', function (req, res) {
  var body = ""
  req.on('data', function (data) {
    body += data
  })

  req.on('end', function () {
    log(req, body)

    console.log("redirecting to :" + req.query.redirect_uri);
    var url = req.query.redirect_uri + "?code=abc&state=" + req.query.state
    res.redirect(url)
  })

})

/*
  once authorization code is given server will call this to get a token.
*/
app.post('/token', function (req, res) {
  console.log('in post<token>...')

  var body = ''
  req.on('data', function (data) {
    body += data
  })

  req.on('end', function (data) {
    log(req, body)

    var tokenResponse = {
      access_token: 'accesstokenhere',
      token_type: 'authorization',
      expires_in: 3600,
      refresh_token: 'refreshtokenhere'
    }

    console.log("sending the response" + JSON.stringify(tokenResponse))
    res.status(200).json(tokenResponse)
  })

})

app.get('/user', function (req, res) {

  req.on('data', function (data) {
  })

  req.on('end', function (data) {
    var userResponse = {
      id: 'demouser',
      email: 'demouser@abc.com'
    }
    console.log("sending the response" + JSON.stringify(userResponse))
    res.status(200).json(userResponse)
  })
})

app.listen(5001, function () {
  console.log("listening at 5001...")
})

function log(req, body) {
  console.log("--------------")
  console.log(body)
  console.log("--------------")

  console.log("------ url --------")
  console.log(req.url)
  console.log("--------------")

  console.log("------ query--------")
  console.log(req.query)
  console.log("--------------")
}
