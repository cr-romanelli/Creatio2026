create or alter view TrnCVwContactAgeDays
as
select Id as TrnCId, Name as TrnCName, BirthDate as TrnCBirthDate,
datediff(day, BirthDate, getdate()) as TrnCAgeDays,
	Id as TrnCContactId
from Contact