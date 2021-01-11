# dotnet_rest_startup_project


To add a new model:

Go to the Data project in the Models folder and add a new class
Add the model as a dbset into mydbcontext class

In a terminal go to the folder of the Data project
Run
dotnet ef --startup-project ./../DataService migrations add [migration_name]
This will create a new migration

dotnet ef --startup-project ./../DataService database update
This will push all new migrations to the database

To add a new rest model in the DataService project
Go to startup.cs
Find call to AddRestModel - and make another one simmilar for your model

To override a generic rest model, create a custom controller class called
[model_name]Controller inheriting from GenericController<[model_name]> and add your code there or override existing methods.