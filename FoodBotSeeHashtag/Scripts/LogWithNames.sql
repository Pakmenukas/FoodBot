select l.Date, u.Name, l.Command, l.Success, l.Data
from Logs l
left join Users u on u.Id = l.UserId
Order by l.Date desc
