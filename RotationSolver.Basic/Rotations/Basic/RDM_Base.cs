using Dalamud.Game.ClientState.JobGauge.Types;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Basic.Actions;
using RotationSolver.Basic.Attributes;
using RotationSolver.Basic.Data;
using RotationSolver.Basic.Helpers;
using RotationSolver.Rotations.CustomRotation;
using static FFXIVClientStructs.FFXIV.Client.UI.Misc.ConfigModule;

namespace RotationSolver.Basic.Rotations.Basic;

public abstract class RDM_Base : CustomRotation
{
    private static RDMGauge JobGauge => Service.JobGauges.Get<RDMGauge>();

    public override MedicineType MedicineType => MedicineType.Intelligence;


    /// <summary>
    /// ��ħԪ
    /// </summary>
    protected static byte WhiteMana => JobGauge.WhiteMana;

    /// <summary>
    /// ��ħԪ
    /// </summary>
    protected static byte BlackMana => JobGauge.BlackMana;

    /// <summary>
    /// ħ����
    /// </summary>
    protected static byte ManaStacks => JobGauge.ManaStacks;

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.RedMage };
    protected override bool CanHealSingleSpell => DataCenter.PartyMembers.Count() == 1 && base.CanHealSingleSpell;

    private sealed protected override IBaseAction Raise => Verraise;

    /// <summary>
    /// �ิ��
    /// </summary>
    public static IBaseAction Verraise { get; } = new BaseAction(ActionID.Verraise, true);

    /// <summary>
    /// ��
    /// </summary>
    public static IBaseAction Jolt { get; } = new BaseAction(ActionID.Jolt)
    {
        StatusProvide = Swiftcast.StatusProvide.Union(new[] { StatusID.Acceleration }).ToArray(),
    };

    /// <summary>
    /// �ش�
    /// </summary>
    public static IBaseAction Riposte { get; } = new BaseAction(ActionID.Riposte)
    {
        ActionCheck = b => JobGauge.BlackMana >= 20 && JobGauge.WhiteMana >= 20,
    };

    /// <summary>
    /// ������
    /// </summary>
    public static IBaseAction Verthunder { get; } = new BaseAction(ActionID.Verthunder)
    {
        StatusNeed = Jolt.StatusProvide,
    };

    /// <summary>
    /// �̱����
    /// </summary>
    public static IBaseAction CorpsACorps { get; } = new BaseAction(ActionID.CorpsACorps, shouldEndSpecial: true)
    {
        ChoiceTarget = TargetFilter.FindTargetForMoving,
    };

    /// <summary>
    /// �༲��
    /// </summary>
    public static IBaseAction Veraero { get; } = new BaseAction(ActionID.Veraero)
    {
        StatusNeed = Jolt.StatusProvide,
    };

    /// <summary>
    /// ɢ��
    /// </summary>
    public static IBaseAction Scatter { get; } = new BaseAction(ActionID.Scatter)
    {
        StatusNeed = Jolt.StatusProvide,
        AOECount = 2,
    };

    /// <summary>
    /// ������
    /// </summary>
    public static IBaseAction Verthunder2 { get; } = new BaseAction(ActionID.Verthunder2)
    {
        StatusProvide = Jolt.StatusProvide,
    };

    /// <summary>
    /// ���ҷ�
    /// </summary>
    public static IBaseAction Veraero2 { get; } = new BaseAction(ActionID.Veraero2)
    {
        StatusProvide = Jolt.StatusProvide,
    };

    /// <summary>
    /// �����
    /// </summary>
    public static IBaseAction Verfire { get; } = new BaseAction(ActionID.Verfire)
    {
        StatusNeed = new[] { StatusID.VerfireReady },
        StatusProvide = Jolt.StatusProvide,
    };

    /// <summary>
    /// ���ʯ
    /// </summary>
    public static IBaseAction Verstone { get; } = new BaseAction(ActionID.Verstone)
    {
        StatusNeed = new[] { StatusID.VerstoneReady },
        StatusProvide = Jolt.StatusProvide,
    };

    /// <summary>
    /// ����ն
    /// </summary>
    public static IBaseAction Zwerchhau { get; } = new BaseAction(ActionID.Zwerchhau)
    {
        ActionCheck = b => BlackMana >= 15 && WhiteMana >= 15,
    };

    /// <summary>
    /// ����
    /// </summary>
    public static IBaseAction Engagement { get; } = new BaseAction(ActionID.Engagement);

    /// <summary>
    /// �ɽ�
    /// </summary>
    public static IBaseAction Fleche { get; } = new BaseAction(ActionID.Fleche);

    /// <summary>
    /// ����
    /// </summary>
    public static IBaseAction Redoublement { get; } = new BaseAction(ActionID.Redoublement)
    {
        ActionCheck = b => BlackMana >= 15 && WhiteMana >= 15,
    };


    /// <summary>
    /// �ٽ�
    /// </summary>
    public static IBaseAction Acceleration { get; } = new BaseAction(ActionID.Acceleration, true)
    {
        StatusProvide = new[] { StatusID.Acceleration },
    };

    /// <summary>
    /// ��Բն
    /// </summary>
    public static IBaseAction Moulinet { get; } = new BaseAction(ActionID.Moulinet)
    {
        ActionCheck = b => BlackMana >= 20 && WhiteMana >= 20,
    };

    /// <summary>
    /// ������
    /// </summary>
    public static IBaseAction Vercure { get; } = new BaseAction(ActionID.Vercure, true)
    {
        StatusProvide = Swiftcast.StatusProvide.Union(Acceleration.StatusProvide).ToArray(),
    };

    /// <summary>
    /// ���ַ���
    /// </summary>
    public static IBaseAction ContreSixte { get; } = new BaseAction(ActionID.ContreSixte);

    /// <summary>
    /// ����
    /// </summary>
    public static IBaseAction Embolden { get; } = new BaseAction(ActionID.Embolden, true);

    /// <summary>
    /// ����
    /// </summary>
    public static IBaseAction MagickBarrier { get; } = new BaseAction(ActionID.MagickBarrier, true, isTimeline: true);

    /// <summary>
    /// ��˱�
    /// </summary>
    public static IBaseAction Verflare { get; } = new BaseAction(ActionID.Verflare);

    /// <summary>
    /// ����ʥ
    /// </summary>
    public static IBaseAction Verholy { get; } = new BaseAction(ActionID.Verholy);

    /// <summary>
    /// ����
    /// </summary>
    public static IBaseAction Scorch { get; } = new BaseAction(ActionID.Scorch)
    {
        ComboIds = new[] { ActionID.Verholy },
    };

    /// <summary>
    /// ����
    /// </summary>
    public static IBaseAction Resolution { get; } = new BaseAction(ActionID.Resolution);

    /// <summary>
    /// ħԪ��
    /// </summary>
    public static IBaseAction Manafication { get; } = new BaseAction(ActionID.Manafication)
    {
        ActionCheck = b => WhiteMana <= 50 && BlackMana <= 50 && InCombat && ManaStacks == 0,
        ComboIdsNot = new[] { ActionID.Riposte, ActionID.Zwerchhau, ActionID.Scorch, ActionID.Verflare, ActionID.Verholy },
    };

    [RotationDesc(ActionID.Vercure)]
    protected sealed override bool HealSingleGCD(out IAction act)
    {
        if (Vercure.CanUse(out act, CanUseOption.MustUse)) return true;
        return false;
    }

    [RotationDesc(ActionID.CorpsACorps)]
    protected sealed override bool MoveForwardAbility(byte abilitiesRemaining, out IAction act, CanUseOption option = CanUseOption.None)
    {
        if (CorpsACorps.CanUse(out act, CanUseOption.EmptyOrSkipCombo | option)) return true;
        return false;
    }

    [RotationDesc(ActionID.Addle, ActionID.MagickBarrier)]
    protected sealed override bool DefenseAreaAbility(byte abilitiesRemaining, out IAction act)
    {
        if (Addle.CanUse(out act)) return true;
        if (MagickBarrier.CanUse(out act, CanUseOption.MustUse)) return true;
        return false;
    }
}
