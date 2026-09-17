using CSharpFunctionalExtensions;
using EmployeeOnboarding.Externals;
using EmployeeOnboarding.Models;

namespace EmployeeOnboarding;

public class Onboarding(IEmployeeRepository employees, IHrSystem hr, IItProvisioning it, IPayroll payroll)
{
    public Result<OnboardingResult, Error> OnboardNewHire(AcceptedOffer offer) 
        => employees.Register(offer)
            .Bind(GenerateContract())
            .Bind(ProvisionAccount())
            .Bind(Enroll());

    private Func<Employee, Result<Contract, Error>> GenerateContract() => hr.GenerateContract;
    private Func<Contract, Result<Account, Error>> ProvisionAccount() => it.ProvisionAccount;
    private Func<Account, Result<OnboardingResult, Error>> Enroll() => payroll.Enroll;
}