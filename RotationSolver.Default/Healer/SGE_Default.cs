namespace RotationSolver.Default.Healer;

[SourceCode("https://github.com/ArchiDog1998/RotationSolver/blob/main/RotationSolver.Default/Healer/SGE_Default.cs")]
public sealed class SGE_Default : SGE_Base
{
    public override string GameVersion => "6.18";

    public override string RotationName => "Default";


    /// <summary>
    /// ���þ������
    /// </summary>
    private static BaseAction MEukrasianDiagnosis { get; } = new(ActionID.EukrasianDiagnosis, true)
    {
        ChoiceTarget = (Targets, mustUse) =>
        {
            var targets = Targets.GetJobCategory(JobRole.Tank);
            if (!targets.Any()) return null;
            return targets.First();
        },
        ActionCheck = b =>
        {
            if (InCombat) return false;
            if (b == Player) return false;
            if (b.HasStatus(false, StatusID.EukrasianDiagnosis, StatusID.EukrasianPrognosis, StatusID.Galvanize)) return false;
            return true;
        }
    };

    protected override bool CanHealSingleSpell => base.CanHealSingleSpell && (Configs.GetBool("GCDHeal") || PartyHealers.Count() < 2);
    protected override bool CanHealAreaSpell => base.CanHealAreaSpell && (Configs.GetBool("GCDHeal") || PartyHealers.Count() < 2);

    protected override IRotationConfigSet CreateConfiguration()
    {
        return base.CreateConfiguration().SetBool("GCDHeal", false, "Auto Use GCD to heal.");
    }

    protected override bool AttackAbility(byte abilitiesRemaining, out IAction act)
    {
        act = null!;
        return false;
    }

    protected override bool EmergencyAbility(byte abilitiesRemaining, IAction nextGCD, out IAction act)
    {
        if (base.EmergencyAbility(abilitiesRemaining, nextGCD, out act)) return true;

        //�¸�������
        if (nextGCD.IsTheSameTo(false, Pneuma, EukrasianDiagnosis,
            EukrasianPrognosis, Diagnosis, Prognosis))
        {
            //�
            if (Zoe.CanUse(out act)) return true;
        }

        if (nextGCD == Diagnosis)
        {
            //���
            if (Krasis.CanUse(out act)) return true;
        }

        return base.EmergencyAbility(abilitiesRemaining, nextGCD, out act);
    }

    [RotationDesc(ActionID.Haima, ActionID.Taurochole)]
    protected override bool DefenseSingleAbility(byte abilitiesRemaining, out IAction act)
    {
        if (Addersgall == 0 || Dyskrasia.CanUse(out _))
        {
            if (Haima.CanUse(out act)) return true;
        }

        //��ţ��֭
        if (Taurochole.CanUse(out act) && Taurochole.Target.GetHealthRatio() < 0.8) return true;

        return base.DefenseSingleAbility(abilitiesRemaining, out act);
    }

    [RotationDesc(ActionID.EukrasianDiagnosis)]
    protected override bool DefenseSingleGCD(out IAction act)
    {
        //���
        if (EukrasianDiagnosis.CanUse(out act))
        {
            if (EukrasianDiagnosis.Target.HasStatus(false,
                StatusID.EukrasianDiagnosis,
                StatusID.EukrasianPrognosis,
                StatusID.Galvanize
            )) return false;

            //����
            if (Eukrasia.CanUse(out act)) return true;

            act = EukrasianDiagnosis;
            return true;
        }

        return base.DefenseSingleGCD(out act);
    }

    [RotationDesc(ActionID.Panhaima, ActionID.Kerachole, ActionID.Holos)]
    protected override bool DefenseAreaAbility(byte abilityRemain, out IAction act)
    {
        //����Ѫ
        if (Addersgall == 0 && PartyMembersAverHP < 0.7)
        {
            if (Panhaima.CanUse(out act)) return true;
        }

        //�����֭
        if (Kerachole.CanUse(out act)) return true;

        //������
        if (Holos.CanUse(out act)) return true;

        return base.DefenseAreaAbility(abilityRemain, out act);
    }

    [RotationDesc(ActionID.EukrasianPrognosis)]
    protected override bool DefenseAreaGCD(out IAction act)
    {
        //Ԥ��
        if (EukrasianPrognosis.CanUse(out act))
        {
            if (EukrasianDiagnosis.Target.HasStatus(false,
                StatusID.EukrasianDiagnosis,
                StatusID.EukrasianPrognosis,
                StatusID.Galvanize
            )) return false;

            //����
            if (Eukrasia.CanUse(out act)) return true;

            act = EukrasianPrognosis;
            return true;
        }

        return base.DefenseAreaGCD(out act);
    }

    protected override bool GeneralAbility(byte abilitiesRemaining, out IAction act)
    {
        //�Ĺ�
        if (Kardia.CanUse(out act)) return true;

        //����
        if (Addersgall == 0 && Rhizomata.CanUse(out act)) return true;

        //����
        if (Soteria.CanUse(out act) && PartyMembers.Any(b => b.HasStatus(true, StatusID.Kardion) && b.GetHealthRatio() < Service.Config.HealthSingleAbility)) return true;

        //����
        if (Pepsis.CanUse(out act)) return true;

        act = null!;
        return false;
    }

    protected override bool GeneralGCD(out IAction act)
    {
        //���� ��һ����λ
        if (Phlegma3.CanUse(out act, mustUse: true, emptyOrSkipCombo: IsMoving || Dyskrasia.CanUse(out _))) return true;
        if (!Phlegma3.EnoughLevel && Phlegma2.CanUse(out act, mustUse: true, emptyOrSkipCombo: IsMoving || Dyskrasia.CanUse(out _))) return true;
        if (!Phlegma2.EnoughLevel && Phlegma.CanUse(out act, mustUse: true, emptyOrSkipCombo: IsMoving || Dyskrasia.CanUse(out _))) return true;

        //ʧ��
        if (Dyskrasia.CanUse(out act)) return true;

        if (EukrasianDosis.CanUse(out var enAct))
        {
            //����Dot
            if (Eukrasia.CanUse(out act)) return true;
            act = enAct;
            return true;
        }

        //עҩ
        if (Dosis.CanUse(out act)) return true;

        //����
        if (Toxikon.CanUse(out act, mustUse: true)) return true;

        //��ս��Tˢ�����ζ���
        if (MEukrasianDiagnosis.CanUse(out _))
        {
            //����
            if (Eukrasia.CanUse(out act)) return true;

            act = MEukrasianDiagnosis;
            return true;
        }
        if (Eukrasia.CanUse(out act)) return true;

        return false;
    }

    [RotationDesc(ActionID.Taurochole, ActionID.Druochole, ActionID.Holos, ActionID.Physis, ActionID.Panhaima)]
    protected override bool HealSingleAbility(byte abilitiesRemaining, out IAction act)
    {
        //��ţ��֭
        if (Taurochole.CanUse(out act)) return true;

        //������֭
        if (Druochole.CanUse(out act)) return true;

        //����Դ����ʱ���뷶Χ���ƻ���ѹ��
        var tank = PartyTanks;
        var isBoss = Dosis.Target.IsBoss();
        if (Addersgall == 0 && tank.Count() == 1 && tank.Any(t => t.GetHealthRatio() < 0.6f) && !isBoss)
        {
            //������
            if (Holos.CanUse(out act)) return true;

            //����
            if (Physis.CanUse(out act)) return true;

            //����Ѫ
            if (Panhaima.CanUse(out act)) return true;
        }

        return base.HealSingleAbility(abilitiesRemaining, out act);
    }

    [RotationDesc(ActionID.Diagnosis)]
    protected override bool HealSingleGCD(out IAction act)
    {
        if (Diagnosis.CanUse(out act)) return true;
        return false;
    }

    [RotationDesc(ActionID.Pneuma, ActionID.Prognosis, ActionID.EukrasianPrognosis)]
    protected override bool HealAreaGCD(out IAction act)
    {
        if (PartyMembersAverHP < 0.65f || Dyskrasia.CanUse(out _) && PartyTanks.Any(t => t.GetHealthRatio() < 0.6f))
        {
            //�����Ϣ
            if (Pneuma.CanUse(out act, mustUse: true)) return true;
        }

        //Ԥ��
        if (EukrasianPrognosis.Target.HasStatus(false, StatusID.EukrasianDiagnosis, StatusID.EukrasianPrognosis, StatusID.Galvanize))
        {
            if (Prognosis.CanUse(out act)) return true;
        }

        if (EukrasianPrognosis.CanUse(out _))
        {
            //����
            if (Eukrasia.CanUse(out act)) return true;

            act = EukrasianPrognosis;
            return true;
        }

        act = null;
        return false;
    }

    [RotationDesc(ActionID.Kerachole, ActionID.Physis, ActionID.Holos, ActionID.Ixochole)]
    protected override bool HealAreaAbility(byte abilitiesRemaining, out IAction act)
    {
        //�����֭
        if (Kerachole.CanUse(out act) && Level >= 78) return true;

        //����
        if (Physis.CanUse(out act)) return true;

        //������
        if (Holos.CanUse(out act) && PartyMembersAverHP < 0.65f) return true;

        //������֭
        if (Ixochole.CanUse(out act)) return true;

        return false;
    }
}