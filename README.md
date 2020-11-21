# gRPC w/ .NET 5.0

This a stock ticker micro services application demo;
- Service - generates randomised stock price changes.
- Clients (A->D) consume the stock price information in a number of different gRPC manners.

## Setup

To run the application demo in Visual Studio 2019+ right-click on the solution in Solution Explorer and select 'Set Startup Projects...' then choose the 'Multiple startup projects' option and then re-arrange the projects in the following order;

- Service
- ClientA
- ClientB
- ClientC
- ClientD

Then hit play...