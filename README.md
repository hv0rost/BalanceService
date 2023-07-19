# BalanceService

## Для запуска приложения необходимо сделать набор действий:
1. Зайти в **BalanceService/BalanceService/appsettings.json** и изменить строку для подключения к БД на актуальную;
2. Открыть скрипт **schema.sql** и создать новую БД;
3. В файле **BalanceService/BalanceService/Controllers/DbController.cs** изменить адрес RabbitMQ по необходимости (212 строка);
4. По желанию изменить строку подключения к БД в **BalanceService/UnitTestBalance/UnitTests.cs** (12 строка);
5. Запустить приложение;

## Маршруты

**GET http://localhost:5100/balance/{id}** имеет необязательный query параметр **currency=USD**, где USD - аббревиатура выводимой валюты
Также возможны варианты - **AUD, AZN, GBP, AMD, BYN, BGN, BRL, EUR и т.д**.
Также можно указать **id** для вывода счета определенного пользователя.

**PUT http://localhost:5100/balance/depositeMoney** - зачисляет деньги на счет.
Тело запроса:

```
{
  "id" : 1,
  "balance" : 100
}
```

Если указанного id, не существует - создает нового пользователя и зачисляет ему на счет указанную сумму денег. 

**PUT http://localhost:5100/balance/chargeMoney** - списывает деньги со счета.
Тело запроса:

```
{
  "id" : 1,
  "balance" : 100
}
```

**PUT http://localhost:5100/balance/transferMoney** - перевод средств с одного счета на другой.
Тело запроса:

```
{
    "to" : 1,
    "from" : 3,
    "moneyAmount" : 750
}
```

**to** - на какой счет

**from** - с какого счета

**moneyAmount** - количество денег

**GET http://localhost:5100/transferHistory/{id}/?sortBy=moneyAmount&page=2** - просмотр транзакций пользователя имеет необязательный query параметр **sortBy={date\moneyAmount}**, 
где **date** - сортировка по дате транзакции, а **moneyAmount** - сортировка по величине средств, которые фигурируют в транзакции.
Второй query параметр **page={номер страницы}** для пагинации по данным. Без указания параметра **page** выводятся все записи. При указании 5 штук на страницу.
Параметр **id** обязательный.

        
