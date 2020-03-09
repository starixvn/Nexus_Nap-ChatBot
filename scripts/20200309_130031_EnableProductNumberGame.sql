/*
	Creator: Oliver@20200309
    Task: Update product Number Game, set IsDisabled = 0  - Redmine: #130031
    
*/

UPDATE Product
SET
    IsDisabled = 0
WHERE ProductName = 'Number Game';

/*<info serverAlias="DBALIAS-Database"></info>*/