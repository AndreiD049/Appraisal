# Appraisal Application

## Dependencies

1. .Net Core
2. Entity Framework Core
3. SQLite

## Database Schema

```mermaid
classDiagram
class User{
	+Int id
	+String Username
	+String Fullname
}
class AppraisalPeriod{
	+int Id
	+String Name
	+String Status
}
AppraisalItem <|--User
AppraisalItem <|--AppraisalPeriod
class AppraisalItem{
	+int Id
	+String Status
	+String Type
	+String Content
	+int AppraisalPeriodId
	+int UserId
}
```

Type of an AppraisalItem can be: Planned, Achieved, SWOT_Strength, SWOT_Weakness, SWOT_Thread, SWOT_Opportunity, Training, Training_Suggested