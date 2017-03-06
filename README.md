# API Documentation

The endpoint of the API for this project is:

```xml
https://auctionitapi.azurewebsites.net/api/...
```

The remainder of the URL will vary based on the operation being performed.

Most API functions require a key that is returned by the API upon successfully logging in via either the web or mobile apps.

## API Calls Reference

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
/auctions/{key}/current
```

**Web Only**

Returns the `Auction` objects for all auctions currently in progress.

---

```xml
/auctions/{key}/upcoming
```

**Web Only**

Returns the same information as the above, but for auctions in the system that have not yet started. Some values will not be useful (0, empty, null, etc.) if the auction has not started.

---

```xml
/auctions/{key}/{id}
```

**Web and Mobile**

Returns the same information as the above, but for one specific auction determined by the value of the ID parameter. Buyers do not have access to information on other buyers.

---

```xml
/auctions/{key}/{auction-id}/bid/{lot-id}/{amount}
```

**Mobile Only**

Places a bid for a lot, specified by its ID, at an auction, also specified by its ID. Returns a `Bid` object indicating whether the bid was recorded successfully or there were any errors. The key will be double-checked to verify that the user is registered for this auction.

---

```xml
/auctions/{key}/states
```

**Web Only**

Returns `Auction` objects sorted into an array of `States`, with appropriate totals information for generating reports based on auction results in different states.

---

```xml
/auctions/{key}/models
```

**Web Only**

Returns `Lot` objects sorted into an array of `Models`, with appropriate totals information for generating reports based on auction results pertaining to different vehicle models.

---

```xml
/auctions/{key}/types
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
/accounts/{key}/create
```

**Web Only**

Creates a new credit account using POST data provided with the API call. Returns a boolean and string confirming the creation of the account, or specifying which error has occurred.

---

```xml
/accounts/{key}/{id}/addbuyer
```

**Web Only**

Creates a new buyer profile using POST data and associates it with the account specified by the ID parameter. Returns the new user login information to be displayed in the website.

---

```xml
/accounts/{key}/{id}/edit
```

**Web Only**

Edits the account specified by the ID variable using POST data.

---

```xml
/accounts/{key}/rankings
```

**Web Only**

Returns an array of `Accounts` sorted in descending order by total spent at auctions to date.

## API Objects Reference

```csharp
namespace AuctionItApi {
    
    interface IAccount {
        string Address;
        int AvailableCredit;
        Buyer[] Buyers;
        string City;
        string ContactEmail;
        string ContactPhone;
        int ID;
        string Owner;
        string PostalCode;
        string State;
        string StateCode;
        int TotalCredit;
        int TotalSpent;
        int UsedCredit;
    }
    
    interface IAuction {
        string Address;
        Buyer[] Buyers; // Not serialized for mobile users
        string City;
        DateTime EndTime;
        int ID;
        Lot[] Lots;
        int Participants;
        string PostalCode;
        DateTime StartTime;
        string State;
        string StateCode;
    }
    
    interface IBid {
        int Amount;
        DateTime BidTime;
        int BuyerID;
        int ID;
        int LotID;
        string Status; // "Placed" "Winner" "Outbid" "Low" "Late" or "Duplicate"
    }
    
    interface IBuyer {
        CreditAccount Account;
        int AuctionCount;
        Bid[] Bids;
        int BidsCount;
        int BidsMax;
        int BidsMin;
        int ID; // Buyers table
        string FirstName;
        string FullName;
        string LastName;
        int TotalSpent;
        string Username;
    }
    
    interface ILot {
        int AuctionID;
        Bid[] Bids;
        Bid BidsMax;
        string Color;
        string Desc; // Year + Make + Model + Trim
        string DetailLink;
        int ID;
        string Make;
        int MinPrice;
        string Model;
        string Status;
        string Trim;
        Bid Winner;
        int VehicleID;
        string VIN;
        int Year;
    }
    
    interface IModel {
        int AvgPrice;
        Lot[] Lots;
        string Make;
        string Name;
        string TotalRevenue;
        int VehiclesSold;
    }
    
    interface IState {
        int AvgPrice;
        Auction[] Auctions;
        string Code;
        string Name;
        int Participants;
        string TotalRevenue;
        int VehiclesSold;
    }
    
    interface IType {
        int AvgPrice;
        Lot[] Lots;
        string Name;
        string TotalRevenue;
        int VehiclesSold;
    }
    
}
```
