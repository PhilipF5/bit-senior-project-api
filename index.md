# Auction It!

This is the documentation page for an automobile auction participation system designed with inspiration from CarMax, Inc. **This project is not officially endorsed or licensed by CarMax, Inc.** It is intended to fulfill the requirements for the capstone project of the Business Information Technology undergraduate program at Virginia Tech, with whom CarMax, Inc. partnered to provide project ideas for the Spring 2017 semester.

## Team Members

* Brandon Kim — Project Manager and Mobile Developer
* James Jae Youn Kim — Unstructured Data Manager and Quality Assurance Analyst
* Kyung Min Lee — Process Manager and Business Intelligence Analyst
* Philip Fulgham — Web Developer and Structured Data Manager

## Web Technologies Info

The web app for this project uses Angular (formerly known as Angular 2), with styles written using the Less CSS preprocessor, web fonts provided by Adobe Typekit, dates and times parsed by Moment Timezone (a variant of Moment.js), and data-driven graphics from Google Charts. The API uses a Microsoft Azure resource group hosting an ASP.NET Core Web API and an Azure SQL Database.

## API Documentation

The endpoint of the API for this project is:

```xml
https://auctionitapi.azurewebsites.net/api/...
```

The remainder of the URL will vary based on the operation being performed.

Most API functions require a key that is returned by the API upon successfully logging in via either the web or mobile apps.

### API Calls Reference

API methods currently available for use on the server are marked with a ![Live](http://www.philipfulgham.name/assets/live.png) tag.

```xml
/login/mobile
```

**Mobile Only**

Sends login credentials for the mobile app via POST. The API will check the credentials against the Users table in the database. If the credentials match, the user's API key is returned to the mobile app, and this key will be used to authenticate further API calls.

---

```xml
/login/web
```

**Web Only**

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
/auctions/bid/{key}/{lot-id}/{amount}
```

**Mobile Only**

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

**Web and Mobile**

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
| Address | string | Street address for the account contact |
| AvailableCredit | decimal | Amount of credit with outstanding bids subtracted |
| Buyers | array | **Web Only** — Buyer objects for each buyer sharing the account |
| City | string | City field for the account contact's mailing address |
| ContactEmail | string | (*Optional*) Account contact email address |
| ContactPhone | string | (*Optional*) Account contact phone number |
| ID | int | ID number of the account in the database |
| Owner | string | Name of the company that owns the account |
| PostalCode | string | Postal code field for the account contact's mailing address |
| State | string | Full name of the state for the account contact's mailing address |
| StateCode | string | Two-letter abbreviation of the state for the account contact's mailing address |
| TotalCredit | decimal | Total amount of credit assigned to the account |
| TotalSpent | decimal | Total amount of money spent at all auctions by members of the account |
| UsedCredit | decimal | Amount of credit currently tied up in outstanding bids |

---

### Auction

| Property | Type | Value |
| -------- | ---- | ----- |
| Address | string | Street address of the auction house |
| Buyers | array | **Web Only** — Buyer objects for each buyer participating in the auction |
| City | string | City of the auction house |
| EndTime | string | UTC date/time the auction will end, in [ISO 8601](https://en.wikipedia.org/wiki/ISO_8601) format |
| ID | int | ID number of the auction in the database |
| Lots | array | Lot objects for each lot available in this auction |
| Participants | int | Number of buyers participating in the auction |
| PostalCode | string | Postal code of the auction house |
| StartTime | string | UTC date/time the auction will start, in [ISO 8601](https://en.wikipedia.org/wiki/ISO_8601) format |
| State | string | Full name of the state of the auction house |
| StateCode | string | Two-letter abbreviation of the state of the auction house |

---

### Bid

| Property | Type | Value |
| -------- | ---- | ----- |
| Amount | decimal | Dollar value of this bid |
| BidTime | string | UTC date/time the bid was placed, in [ISO 8601](https://en.wikipedia.org/wiki/ISO_8601) format |
| BuyerID | int | ID number of the buyer in the database |
| ID | int | ID number of the bid in the database |
| LotID | int | ID number of lot in the database |
| Status | string | `Placed` — highest valid bid in the system; `Winner` — bid accepted by the auction manager; `Outbid` — bid no longer winning, funds have been released; `Low` — bid invalid because a higher bid already existed; `Late` — bid invalid because another bid has already won; `Duplicate` — bid invalid because current highest bid is from the same credit account |

---

### Buyer

| Property | Type | Value |
| -------- | ---- | ----- |
| AccountID | int | ID number of the credit account in the database |
| AuctionCount | int | Number of auctions the buyer has participated in |
| Bids | array | Bid objects for each bid the buyer has ever placed |
| BidsCount | int | Number of bids the buyer has placed |
| BidsMax | decimal | Highest bid the buyer has ever placed |
| BidsMin | decimal | Lowest bid the buyer has ever placed |
| ID | int | ID number of the buyer in the database |
| FirstName | string | First name of the buyer |
| FullName | string | First and last name of the buyer |
| LastName | string | Last name of the buyer |
| TotalSpent | decimal | Total amount of money spent at all auctions by the buyer |
| Username | string | Login username for the buyer |

---

### Lot

| Property | Type | Value |
| -------- | ---- | ----- |
| AuctionID | int | ID number of the auction in the database |
| Bids | array | Bid objects for each bid placed for the lot |
| BidsMax | Bid | Highest bid placed for the lot |
| Color | string | Color of the vehicle |
| Desc | string | Year, Make, Model, and Trim of the vehicle — e.g., "2012 Ford Fusion SEL" |
| DetailLink | string | URL for unstructured data page associated with the vehicle |
| ID | int | ID number of the lot in the database |
| Make | string | Make of the vehicle |
| MinPrice | int | Starting price of the lot |
| Model | string | Model of the vehicle |
| Status | string | `Sold` — lot has been sold; `Unsold` — lot is open for bidding, or auction has not started |
| Trim | string | Trim package of the vehicle |
| Winner | Bid | (*Optional*) Winning bid for the lot |
| VehicleID | int | ID number of the vehicle in the database |
| VIN | string | [Vehicle Identification Number](https://en.wikipedia.org/wiki/Vehicle_identification_number) assigned by the manufacturer |
| Year | int | Model year of the vehicle |
