# Auction It!

This is the API documentation page for an automobile auction participation system designed with inspiration from CarMax, Inc. **This project is not officially endorsed or licensed by CarMax, Inc.** It is intended to fulfill the requirements for the capstone project of the Business Information Technology undergraduate program at Virginia Tech, with whom CarMax, Inc. partnered to provide project ideas for the Spring 2017 semester.

## Team Members

* **Brandon Kim**, Project Manager and Mobile Developer
* **James Jae Youn Kim**, Unstructured Data Manager and Quality Assurance Analyst
* **Kyung Min Lee**, Process Manager and Business Intelligence Analyst
* **Philip Fulgham**, Web Developer and Structured Data Manager

# API Documentation

The endpoint of the API for this project is:

```xml
https://auctionitapi.azurewebsites.net/api/...
```

The remainder of the URL will vary based on the operation being performed.

Most API functions require a key that is returned by the API upon successfully logging in via either the web or mobile apps.

## Your New Best Friend: Auction 11

Auction 11 is a very special auction in the database that is useful for testing methods. Auction 11 (named after its ID number) is an auction that is always current, because it's over two months long (don't worry, the fictional buyers can take fictional meal and bathroom breaks). Why do we have this? Because it gives us an auction that we can always rely on to be currently active, without constantly adding new auctions or adjusting dates in the database.

## API Calls Reference

API methods currently available for use on the server are marked with a ![Live](http://www.philipfulgham.name/assets/live.png) tag.

```xml
/login/mobile
```

![Live](http://www.philipfulgham.name/assets/live.png) **Mobile Only**

Sends login credentials for the mobile app via POST. The credentials should be sent as an `application/json` content-type, formatted as a single string with a space separating the username and password. The API will check the credentials against the Users table in the database and return a `Login` object, which is described below.

---

```xml
/login/web
```

![Live](http://www.philipfulgham.name/assets/live.png) **Web Only**

Same as above, except this handles logins from the web interface. The API checks the credentials against the Managers table instead of the Users table.

---

```xml
/auctions/current/{key}
```

**Web Only**

Returns the `Auction` objects for all auctions currently in progress.

---

```xml
/auctions/upcoming/{key}
```

**Web Only**

Returns the same information as the above, but for auctions in the system that have not yet started. Some values may not be useful (0, empty, null, etc.) if the auction has not started.

---

```xml
/auctions/{key}/{id}
```

![Live](http://www.philipfulgham.name/assets/live.png) **Web and Mobile**

Returns the same information as the above, but for one specific auction determined by the value of the ID parameter. Buyers do not have access to information on other buyers.

---

```xml
/auctions/bid/{key}/{lotID}/{amount}
```

![Live](http://www.philipfulgham.name/assets/live.png) **Mobile Only**

Places a bid for a lot, specified by its ID, at an auction. Returns a `Bid` object indicating whether the bid was recorded successfully or there were any errors. The key will be double-checked to verify that the user is registered for this auction.

---

```xml
/auctions/states/{key}
```

**Web Only**

Returns `Auction` objects sorted into an array of `States`, with appropriate totals information for generating reports based on auction results in different states.

---

```xml
/auctions/models/{key}
```

**Web Only**

Returns `Lot` objects sorted into an array of `Models`, with appropriate totals information for generating reports based on auction results pertaining to different vehicle models.

---

```xml
/auctions/types/{key}
```

**Web Only**

Returns `Lot` objects sorted into an array of `Types`, with appropriate totals information for generating reports based on auction results pertaining to different vehicle types (sedan, truck, etc.).

---

```xml
/accounts/{key}
```

![Live](http://www.philipfulgham.name/assets/live.png) **Web and Mobile**

On web, returns the `Account` objects for all open credit accounts. On mobile, returns the `Account` object associated with the buyer profile key supplied.

---

```xml
/accounts/{key}/{id}
```

**Web Only**

Returns the `Account` object for a single credit account specified by the ID parameter.

---

```xml
/accounts/create/{key}
```

**Web Only**

Creates a new credit account using POST data provided with the API call. Returns a boolean and string confirming the creation of the account, or specifying which error has occurred.

---

```xml
/accounts/addbuyer/{key}/{id}
```

**Web Only**

Creates a new buyer profile using POST data and associates it with the account specified by the ID parameter. Returns the new user login information to be displayed in the website.

---

```xml
/accounts/edit/{key}/{id}
```

**Web Only**

Edits the account specified by the ID variable using POST data.

---

```xml
/accounts/rankings/{key}
```

**Web Only**

Returns an array of `Accounts` sorted in descending order by total spent at auctions to date.

## API Objects Reference

### Account

| Property | Type | Value |
| -------- | ---- | ----- |
| address | string | Street address for the account contact |
| availableCredit | decimal | Amount of credit with outstanding bids subtracted |
| buyers | array | **Web Only** — Buyer objects for each buyer sharing the account |
| city | string | City field for the account contact's mailing address |
| contactEmail | string | (*Optional*) Account contact email address |
| contactPhone | string | (*Optional*) Account contact phone number |
| id | int | ID number of the account in the database |
| owner | string | Name of the company that owns the account |
| postalCode | string | Postal code field for the account contact's mailing address |
| state | string | Full name of the state for the account contact's mailing address |
| stateCode | string | Two-letter abbreviation of the state for the account contact's mailing address |
| totalCredit | decimal | Total amount of credit assigned to the account |
| totalSpent | decimal | Total amount of money spent at all auctions by members of the account |
| usedCredit | decimal | Amount of credit currently tied up in outstanding bids |

---

### Auction

| Property | Type | Value |
| -------- | ---- | ----- |
| address | string | Street address of the auction house |
| buyers | array | **Web Only** — Buyer objects for each buyer participating in the auction |
| city | string | City of the auction house |
| endTime | string | UTC date/time the auction will end, in [ISO 8601](https://en.wikipedia.org/wiki/ISO_8601) format |
| id | int | ID number of the auction in the database |
| lots | array | Lot objects for each lot available in this auction |
| participants | int | Number of buyers participating in the auction |
| postalCode | string | Postal code of the auction house |
| startTime | string | UTC date/time the auction will start, in [ISO 8601](https://en.wikipedia.org/wiki/ISO_8601) format |
| state | string | Full name of the state of the auction house |
| stateCode | string | Two-letter abbreviation of the state of the auction house |

---

### Bid

| Property | Type | Value |
| -------- | ---- | ----- |
| accountID | int | ID number of the credit account in the database |
| amount | decimal | Dollar value of this bid |
| bidTime | string | UTC date/time the bid was placed, in [ISO 8601](https://en.wikipedia.org/wiki/ISO_8601) format |
| buyerID | int | ID number of the buyer in the database |
| id | int | ID number of the bid in the database |
| lotID | int | ID number of lot in the database |
| status | string | `Placed` — highest valid bid in the system; `Winner` — bid accepted by the auction manager; `Outbid` — bid no longer winning, funds have been released; `Low` — bid invalid because a higher bid already existed or did not meet minimum price; `Late` — bid invalid because another bid has already won; `Duplicate` — bid invalid because current highest bid is from the same credit account; `Unauthorized` — bid invalid because buyer is not registered to the auction, all other properties will be 0; `Bounced` — bid invalid because account had insufficient available credit |

---

### Buyer

| Property | Type | Value |
| -------- | ---- | ----- |
| accountID | int | ID number of the credit account in the database |
| auctionCount | int | Number of auctions the buyer has participated in |
| bids | array | Bid objects for each bid the buyer has ever placed |
| bidsCount | int | Number of bids the buyer has placed |
| bidsMax | decimal | Highest bid the buyer has ever placed |
| bidsMin | decimal | Lowest bid the buyer has ever placed |
| id | int | ID number of the buyer in the database |
| firstName | string | First name of the buyer |
| fullName | string | First and last name of the buyer |
| lastName | string | Last name of the buyer |
| totalSpent | decimal | Total amount of money spent at all auctions by the buyer |
| username | string | Login username for the buyer |

---

### Login

| Property | Type | Value |
| -------- | ---- | ----- |
| apiKey | string | API key that can be passed as the `{key}` parameter in an API call |
| error | string | Error message if the login failed, or null if it succeeded |
| firstName | string | First name of the user who is now logged in |
| lastName | string | Last name of the user who is now logged in |
| username | string | Username that was submitted to the API |

---

### Lot

| Property | Type | Value |
| -------- | ---- | ----- |
| auctionID | int | ID number of the auction in the database |
| bids | array | Bid objects for each bid placed for the lot |
| bidsMax | Bid | Highest bid placed for the lot |
| color | string | Color of the vehicle |
| desc | string | Year, Make, Model, and Trim of the vehicle — e.g., "2012 Ford Fusion SEL" |
| detailLink | string | URL for unstructured data page associated with the vehicle |
| id | int | ID number of the lot in the database |
| make | string | Make of the vehicle |
| minPrice | int | Starting price of the lot |
| model | string | Model of the vehicle |
| status | string | `Sold` — lot has been sold; `Unsold` — lot is open for bidding, or auction has not started |
| trim | string | Trim package of the vehicle |
| winner | Bid | (*Optional*) Winning bid for the lot |
| vehicleID | int | ID number of the vehicle in the database |
| vin | string | [Vehicle Identification Number](https://en.wikipedia.org/wiki/Vehicle_identification_number) assigned by the manufacturer |
| year | int | Model year of the vehicle |
