using CSharpFunctionalExtensions;
using EmployeeOnboarding.Externals;
using EmployeeOnboarding.Models;

namespace EmployeeOnboarding;

public class Onboarding(IEmployeeRepository employees, IHrSystem hr, IItProvisioning it, IPayroll payroll)
{
    public Result<OnboardingResult, Error> OnboardNewHire(AcceptedOffer offer)
    {
        try
        {
            var employee = employees.Register(offer);
            var contract = hr.GenerateContract(employee);
            
            return it.ProvisionAccount(contract)
                .Bind(payroll.Enroll);
        }
        catch (EmployeeRegistrationException e)
        {
            throw new BusinessException(e.Message);
        }
        catch (ContractGenerationException e)
        {
            throw new BusinessException(e.Message);
        }
    }
}