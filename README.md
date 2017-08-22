**IMPORTANT: This repo is a college class project that is being developed for course credit. All rights are reserved to the project team. As per GitHub ToS, others may view, fork, and learn from this code, but NO LICENSE is granted to use or redistribute this project or any derivative thereof for any purpose.**

# Virginia Tech BIT Senior Project — CarMax Auction It

This repo hosts an automobile auction participation system designed to fulfill the requirements for the capstone project of the Business Information Technology undergraduate program at Virginia Tech.

## Team Members

* **Brandon Kim**, Project Manager and Mobile Developer
* **James Jae Youn Kim**, Unstructured Data Manager and Quality Assurance Analyst
* **Kyung Min Lee**, Process Manager and Business Intelligence Analyst
* **Philip Fulgham**, Web Developer and Structured Data Manager

## Technology Stack

The front end for this project consists of both web and native apps. These apps are built and deployed using a single codebase by the [Ionic Framework](http://ionicframework.com), which uses [Angular](http://angular.io) (HTML5, [Sass](http://sass-lang.com), and [TypeScript](https://www.typescriptlang.org)) on top of the [Apache Cordova](https://cordova.apache.org) hybrid platform. The apps also make use of the [Chart.js](http://www.chartjs.org), [Moment Timezone](http://momentjs.com/timezone/), and [randomColor](https://randomcolor.llllll.li/) JavaScript libraries. The front end is hosted in a separate repo.

The back end as hosted in this repo is powered by [ASP.NET Core MVC](https://www.asp.net/core) running on the [Microsoft Azure](https://azure.microsoft.com) cloud. The apps communicate with it via a RESTful API, which the underlying C# code uses to query and/or manipulate a PostgreSQL database running on [Heroku Postgres](https://www.heroku.com/postgres).