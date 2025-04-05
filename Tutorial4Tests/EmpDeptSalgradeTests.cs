using Tutorial3.Models;

public class EmpDeptSalgradeTests
{
    // 1. Simple WHERE filter
    // SQL: SELECT * FROM Emp WHERE Job = 'SALESMAN';
    [Fact]
    public void ShouldReturnAllSalesmen()
    {
        var emps = Database.GetEmps();
        var result = emps.Where(e => e.Job == "SALESMAN").ToList();
    }

    // 2. WHERE + OrderBy
    // SQL: SELECT * FROM Emp WHERE DeptNo = 30 ORDER BY Sal DESC;
    [Fact]
    public void ShouldReturnDept30EmpsOrderedBySalaryDesc()
    {
        var emps = Database.GetEmps();
        var result = emps.Where(e => e.Job == "DEPT30").OrderByDescending(e=>e.Sal).ToList();
    }

    // 3. Subquery using LINQ (IN clause)
    // SQL: SELECT * FROM Emp WHERE DeptNo IN (SELECT DeptNo FROM Dept WHERE Loc = 'CHICAGO');
    [Fact]
    public void ShouldReturnEmployeesFromChicago()
    {
        var emps = Database.GetEmps();
        var depts = Database.GetDepts();
        var deptNoList = depts.Where(e => e.Loc == "CHICAGO" ).Select(e=>e.DeptNo).ToList();
        var result = emps.Where(e=> deptNoList.Contains(e.DeptNo)).ToList();
    }

    // 4. SELECT projection
    // SQL: SELECT EName, Sal FROM Emp;
    [Fact]
    public void ShouldSelectNamesAndSalaries()
    {
        var emps = Database.GetEmps();
        var result = emps.Select(e=>new {e.EName,e.Sal}).ToList();
    }

    // 5. JOIN Emp to Dept
    // SQL: SELECT E.EName, D.DName FROM Emp E JOIN Dept D ON E.DeptNo = D.DeptNo;
    [Fact]
    public void ShouldJoinEmployeesWithDepartments()
    {
        var emps = Database.GetEmps();
        var depts = Database.GetDepts();

        var result = depts.Join(emps,
            e => e.DeptNo,
            d => d.DeptNo,
            (d,e) => new
            {e.EName,
                d.DName
            });
    }

    // 6. Group by DeptNo
    // SQL: SELECT DeptNo, COUNT(*) FROM Emp GROUP BY DeptNo;
    [Fact]
    public void ShouldCountEmployeesPerDepartment()
    {
        var emps = Database.GetEmps();
        var result = emps.GroupBy(e => e.DeptNo).Select(
            g=>new
            {
                DeptNo = g.Key,
                EmpCount = g.Count()
            }).ToList();
    }

    // 7. SelectMany (simulate flattening)
    // SQL: SELECT EName, Comm FROM Emp WHERE Comm IS NOT NULL;
    [Fact]
    public void ShouldReturnEmployeesWithCommission()
    {
        var emps = Database.GetEmps();
        var results = emps.Where(e => e.Comm != null).SelectMany(
            e => new[] {new { e.EName, e.Comm }}).ToList();
    }

    // 8. Join with Salgrade
    // SQL: SELECT E.EName, S.Grade FROM Emp E JOIN Salgrade S ON E.Sal BETWEEN S.Losal AND S.Hisal;
    [Fact]
    public void ShouldMatchEmployeeToSalaryGrade()
    {
        var emps = Database.GetEmps();
        var grades = Database.GetSalgrades();
        var result = emps
            .Join(
                grades,
                e => 1, 
                s => 1, 
                (e, s) => new { e, s })
            .Where(es => es.e.Sal >= es.s.Losal && es.e.Sal <= es.s.Hisal)
            .Select(es => new { es.e.EName, es.s.Grade })
            .ToList();

    }

    // 9. Aggregation (AVG)
    // SQL: SELECT DeptNo, AVG(Sal) FROM Emp GROUP BY DeptNo;
    [Fact]
    public void ShouldCalculateAverageSalaryPerDept()
    {
        var emps = Database.GetEmps();
        var result = emps.GroupBy(e => e.DeptNo).Select(
            g =>
            new
            {
                DeptNo = g.Key,
                Avg = g.Average(e => e.Sal)
            }).ToList();
    }

    // 10. Complex filter with subquery and join
    // SQL: SELECT E.EName FROM Emp E WHERE E.Sal > (SELECT AVG(Sal) FROM Emp WHERE DeptNo = E.DeptNo);
    [Fact]
    public void ShouldReturnEmployeesEarningMoreThanDeptAverage()
    {
        var emps = Database.GetEmps();
        var avrageEmps = emps.Average(e => e.Sal);
        var result = emps.Where(e => e.DeptNo > avrageEmps).Select(
            e=> e.EName).ToList();
    }
}
