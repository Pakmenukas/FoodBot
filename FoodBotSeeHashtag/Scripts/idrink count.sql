select u.Name, count(*) skardines
from Logs l
left join Users u on l.UserId = u.Id
where Command = 'idrink' and Success = 1
group by u.Name
order by Date desc