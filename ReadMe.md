# Dapper POC

After many conversations with people from multiple industry types using .NET, a single common technology stack discussed was Dapper. 

Whilst I’ve always been able to achieve consistent and performant results with Entity Framework, I decided to investigate Dapper to see how easy it is to implement and further explore if it could complement or work in conjunction with Entity Framework. Seems like the answer is yes.

This POC is designed to do just that. 

- Mitigating database schema migrations and seeding data to Entity Framework (Code First)
- Initial exploration into simple and slightly more complex queries to experience how Dapper handles searching for single records and moving on to more complex responses that leverage navigational properties. 

## Project 

```
| --Solution-(dapper-poc)-
|
|   Entities
|     Person
|     Faculty
|   Schema 
|     DbContexts
|     Migrations
|   Stub
|     Program
|
| ------------------------
```
> Entities

Class library. Designed to house all the Entities for use with both: 

1. Entity Framework
2. Dapper backing models.

> Schema

Class library. Designed to handle database schema migration with Entity Framework (Code First).

> Stub

1. Brief concrete implementer. Calls Entity Framework to apply schema migrations and data seeds if required.
2. Implements Dapper to execute the following queries:
    - A simple find person by name
    - Slightly more complex `INNER JOIN` query to explore how navigational properties can still be leveraged  

## References:

- https://github.com/DapperLib/Dapper
- https://www.learndapper.com/relationships