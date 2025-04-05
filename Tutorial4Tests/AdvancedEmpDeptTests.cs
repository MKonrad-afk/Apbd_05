namespace Tutorial3Tests;

public class AdvancedEmpDeptTests
{
    // 11. MAX salary
    // SQL: SELECT MAX(Sal) FROM Emp;
    [Fact]
    public void ShouldReturnMaxSalary()
    {
        var emps = Database.GetEmps();
        var result = emps.Max(e => e.Sal);
        Assert.Equal(5000, result);
    }

    // 12. MIN salary in department 30
    // SQL: SELECT MIN(Sal) FROM Emp WHERE DeptNo = 30;
    [Fact]
    public void ShouldReturnMinSalaryInDept30()
    {
        var emps = Database.GetEmps();

        var result = emps.Where(e=> e.DeptNo==30).Min(e => e.Sal);

        Assert.Equal(1250, result);
    }

    // 13. Take first 2 employees ordered by hire date
    // SQL: SELECT * FROM Emp ORDER BY HireDate ASC FETCH FIRST 2 ROWS ONLY;
    [Fact]
    public void ShouldReturnFirstTwoHiredEmployees()
    {
        var emps = Database.GetEmps();

        var result = emps.OrderBy(e=>e.HireDate).Take(2).ToList();
        Assert.Equal(2, result.Count);
        Assert.True(result[0].HireDate <= result[1].HireDate);
    }

    // 14. DISTINCT job titles
    // SQL: SELECT DISTINCT Job FROM Emp;
    [Fact]
    public void ShouldReturnDistinctJobTitles()
    {
        var emps = Database.GetEmps();

        var result= emps.Select(e=>e.Job).Distinct().ToList();
        Assert.Contains("PRESIDENT", result);
        Assert.Contains("SALESMAN", result);
    }

    // 15. Employees with managers (NOT NULL Mgr)
    // SQL: SELECT * FROM Emp WHERE Mgr IS NOT NULL;
    [Fact]
    public void ShouldReturnEmployeesWithManagers()
    {
        var emps = Database.GetEmps();
        var result = emps.Where(e => e.Mgr != null).ToList();
        Assert.All(result, e => Assert.NotNull(e.Mgr));
    }

    // 16. All employees earn more than 500
    // SQL: SELECT * FROM Emp WHERE Sal > 500; (simulate all check)
    [Fact]
    public void AllEmployeesShouldEarnMoreThan500()
    {
        var emps = Database.GetEmps();
        var reselt =  emps.All(e=>e.Sal>500);
        Assert.True(reselt);
    }

    // 17. Any employee with commission over 400
    // SQL: SELECT * FROM Emp WHERE Comm > 400;
    [Fact]
    public void ShouldFindAnyWithCommissionOver400()
    {
        var emps = Database.GetEmps();
        var result = emps.Any(e=>e.Comm>400);

        Assert.True(result);
    }

    // 18. Self-join to get employee-manager pairs
    // SQL: SELECT E1.EName AS Emp, E2.EName AS Manager FROM Emp E1 JOIN Emp E2 ON E1.Mgr = E2.EmpNo;
    [Fact]
    public void ShouldReturnEmployeeManagerPairs()
    {
        var emps = Database.GetEmps();
        var result = emps
            .Join(emps, 
                e1 => e1.Mgr, 
                e2 => e2.EmpNo, 
                (e1, e2) => new 
                {
                    Employee = e1.EName,
                    Manager = e2.EName
                })
            .ToList();
        Assert.Contains(result, r => r.Employee == "SMITH" && r.Manager == "FORD");
    }

    // 19. Let clause usage (sal + comm)
    // SQL: SELECT EName, (Sal + COALESCE(Comm, 0)) AS TotalIncome FROM Emp;
    [Fact]
    public void ShouldReturnTotalIncomeIncludingCommission()
    {
        var emps = Database.GetEmps();
        var result = emps
            .Select(e => new
            {
                e.EName,
                Total = e.Sal + (e.Comm ?? 0)
            })
            .ToList();

        Assert.Contains(result, r => r.EName == "ALLEN" && r.Total == 1900);
    }

    // 20. Join all three: Emp → Dept → Salgrade
    // SQL: SELECT E.EName, D.DName, S.Grade FROM Emp E JOIN Dept D ON E.DeptNo = D.DeptNo JOIN Salgrade S ON E.Sal BETWEEN S.Losal AND S.Hisal;
    [Fact]
    public void ShouldJoinEmpDeptSalgrade()
    {
        var emps = Database.GetEmps();
        var depts = Database.GetDepts();
        var grades = Database.GetSalgrades();
        var result = emps
            .Join(depts, 
                e => e.DeptNo, 
                d => d.DeptNo, 
                (e, d) => new { e, d })
            .SelectMany(
                ed => grades
                    .Where(s => ed.e.Sal >= s.Losal && ed.e.Sal <= s.Hisal),
                (ed, s) => new 
                {
                    EmpName = ed.e.EName,
                    DeptName = ed.d.DName,
                    Grade = s.Grade
                })
            .ToList();
        
        Assert.Contains(result, r => r.EmpName == "ALLEN" && r.DeptName == "SALES" && r.Grade == 3);
    }
}
