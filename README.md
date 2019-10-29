# EfCore-CollectionInExpressionProblem

EFCore 3.0 seems to have problem when translating `ICollection<T>` into query. This repository contains unit tests demonstrating that behavior was different in EfCore 2.2 and current EfCore 3.0.

## How to run

```bash
docker build .
```

## Issue

https://github.com/aspnet/EntityFrameworkCore/issues/18639

