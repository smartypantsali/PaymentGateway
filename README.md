# Payment Gateway Web API

## About:
- API to create and process payments / Get payments. 
- With Authorisation / Authentication and permission setting
- Logging functionality
- Data storage
- Environment configuration

## Assumptions made:
- "Acquiring  Bank" part of the process, is an endpoint to be called. Therefore I created a new endpoint for it and Mocked it by using HttpClient object. This will only return a unique id and 200 status code. I left it as mvp as possible as it is just for mocking.
- Merchant will require Authorisation  / Authentication.
- Masking card numbers to be displayed like "****20" for example.
- Checking the validity of the card by using Luhn algorithm.

## Improvements that can be made:
- Improve logging to be deeper, and log the entire process of endpoint in a context, with less clutter.
- Use a logging tool like "Seq"
- Application metrics
- Encryption

## There are things to keep in mind, as it will be useful to be aware of:
- There are 2 main projects in the solution. "Framework" for reusable code. And "PaymentGateway.WebApi" which contains the endpoints. Each has its test projects as well.
- Database used for application is called LiteDB. If you would like to query the information, please refer to: https://github.com/mbdavid/LiteDB.Studio.
- Logs are stored in a file under "Logs" folder. Endpoint execution can be tracked using "TraceIdentifier" response header, per request.
- Logfile can only be opened once application stops running.
- Some configuration for application, can be found in "appsettings.json"
- Before using payments endpoints, user will need to be Authenticated and cache its associated cookie. Users also have access to 2 permissions "["Payment_Create", "Payment_View"]".
As the only endpoints that allow Anonymous access are User create and login.
- Ready to go endpoints (to be used in postman), can be obtained here: https://gofile.io/d/ExcqoT
- I wrote 65 unit tests, to try and cover all bases. Found in FrameworkTests and PaymentGateway.WebApiTests

# REST API

In case missed, please refer to https://gofile.io/d/ExcqoT, for collection of all endpoints.

## Create User
- Alice can be Anonymous

### Request

`POST https://localhost:44365/user/create`

    {
      "username": "Ali",
      "password": "test",
      "permissions": [
          "Payment_View", 
          "Payment_Create"
      ]
    }

### Responses
    Status: 200 OK
    {
      "uid": "8d8062f8-ae44-4ebc-ad44-b65bf5fd6c7c",
      "username": "Ali",
      "permissions": [
          "Payment_View",
          "Payment_Create"
      ]
    }
    
    Status: 418 I'm a teapot (List of error messages)
    {
      "Username": {
          "ErrorCode": "missing"
      },
      "Password": {
          "ErrorCode": "missing"
      },
      "Password": {
          "ErrorCode": "must_be_at_least_four_characters"
      }
    }

## Login
- Alice can be Anonymous

### Request

`POST https://localhost:44365/user/login`

    {
      "username": "Ali",
      "password": "test"
    }

### Responses
    Status: 200 OK
    {
      "uid": "f9b229ea-faab-4a1c-8b49-abd45a758e52",
      "username": "Ali",
      "permissions": [
          "Payment_Create",
          "Payment_View"
      ]
    }
    
    Status: 418 I'm a teapot (List of error messages)
    {
      "Username": {
          "ErrorCode": "missing"
      },
      "Password": {
          "ErrorCode": "missing"
      }
    }

## Assign Permissions
- Alice requires Authorisation

### Request

`PATCH https://localhost:44365/user/:uid/permissions/assign`

    [ "Payment_Create", "Payment_View"]

### Responses
    Status: 200 OK
    {
      "uid": "f9b229ea-faab-4a1c-8b49-abd45a758e52",
      "username": "Ali",
      "permissions": [
          "Payment_Create",
          "Payment_View"
      ]
    }
    
    Status: 404 Not Found (invalid uid)

## SignOut User
- Alice requires Authorisation

### Request

`GET https://localhost:44365/user/signout`

### Response
    Status: 200 OK

## Get all Users
- Alice requires Authorisation

### Request

`GET https://localhost:44365/user/all`

### Response

    Status: 200 OK
    [
      {
          "uid": "f9b229ea-faab-4a1c-8b49-abd45a758e52",
          "username": "Ali",
          "permissions": [
              "Payment_Create",
              "Payment_View"
          ]
      },
      {
          "uid": "8d8062f8-ae44-4ebc-ad44-b65bf5fd6c7c",
          "username": "Ali1",
          "permissions": [
              "Payment_View",
              "Payment_Create"
          ]
      }
    ]

## Get UserByUid
- Alice requires Authorisation

### Request

`GET https://localhost:44365/user/:uid`

### Response
    Status: 200 OK
    {
      "uid": "f9b229ea-faab-4a1c-8b49-abd45a758e52",
      "username": "Ali",
      "permissions": [
          "Payment_Create",
          "Payment_View"
      ]
    }
    
    Status: 404 Not Found

## Create Payment
- Alice requires Authorisation
- Alice requires Payment_View AND Payment_Create Permissions

### Request

`POST https://localhost:44365/payment`

    {
      "cardHolderName": "MR Twinkle",
      "cardNumber": "4916132996393639",
      "expiryDate": "2020-12-20",
      "amount": 100.23,
      "currency": "GBP",
      "Cvv": "123"
    }

### Responses
    Status: 200 OK
    {
      "uid": "HSBC-3e2c543e-3ef1-42fd-b2bb-044c36dc69f2",
      "cardHolderName": "MR Twinkle",
      "cardNumber": "***********93639",
      "expiryDate": "********20",
      "amount": 100.23,
      "currency": "GBP",
      "cvv": "***",
      "paymentDate": "2020-12-18T13:19:18.6008802+00:00",
      "state": "Completed"
    }
    
    Status: 403 Forbidden
    
    Status: 418 I'm a teapot (List of error messages)
    {
      "CardNumber": {
          "ErrorCode": "missing"
      },
      "Cvv": {
          "ErrorCode": "missing"
      },
      "CardHolderName": {
          "ErrorCode": "missing"
      },
      "ExpiryDate": {
          "ErrorCode": "missing"
      },
      "Amount": {
          "ErrorCode": "missing"
      },
      "Currency": {
          "ErrorCode": "missing"
      },
      "Amount": {
        "ErrorCode": "amount_too_high"
      },
      "Amount": {
        "ErrorCode": "amount_too_low"
      },
      "ExpiryDate": {
        "ErrorCode": "expirydate_in_the_past"
      },
      "ExpiryDate": {
        "ErrorCode": "incorrect_format"
      },
      "CardNumber": {
        "ErrorCode": "invalid_card_number"
      },
      "Cvv": {
          "ErrorCode": "invalid_cvv"
      },
      "Currency": {
        "ErrorCode": "currency_not_supported"
      }
    }

## Get PaymentByUid
- Alice requires Authorisation
- Alice requires Payment_View Permission

### Request

`GET https://localhost:44365/payment/:uid`

### Response

    Status: 200 OK
    {
      "uid": "HSBC-916d879f-1357-4559-830c-67072e71a760",
      "cardHolderName": "MR Twinkle",
      "cardNumber": "***********93639",
      "expiryDate": "********20",
      "amount": 100.23,
      "currency": "GBP",
      "cvv": "***",
      "paymentDate": "2020-12-18T12:34:29.555+00:00",
      "state": "Completed"
    }
    
    Status: 404 Not Found

## Get All Payments
- Alice requires Authorisation
- Alice requires Payment_View Permission

### Request

`GET https://localhost:44365/payment/all`

### Response
    Status: 200 OK
    [
      {
          "uid": "HSBC-916d879f-1357-4559-830c-67072e71a760",
          "cardHolderName": "MR Twinkle",
          "cardNumber": "***********93639",
          "expiryDate": "********20",
          "amount": 100.23,
          "currency": "GBP",
          "cvv": "***",
          "paymentDate": "2020-12-18T12:34:29.555+00:00",
          "state": "Completed"
      },
      {
          "uid": "HSBC-3e2c543e-3ef1-42fd-b2bb-044c36dc69f2",
          "cardHolderName": "MR Twinkle",
          "cardNumber": "***********93639",
          "expiryDate": "********20",
          "amount": 100.23,
          "currency": "GBP",
          "cvv": "***",
          "paymentDate": "2020-12-18T13:19:18.6+00:00",
          "state": "Completed"
      }
    ]
