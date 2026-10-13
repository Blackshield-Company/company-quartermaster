using Modkit.Mutagen;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using static CompanyQuartermaster.Generator.Vanilla;

namespace CompanyQuartermaster.Generator;

internal sealed class Builder
{
    private readonly SkyrimMod _mod;
    private readonly FormIdRegistry _ids;
    private static readonly SkyrimRelease R = SkyrimRelease.SkyrimSE;
    private const byte VanillaQuestFormVersion = 65;
    public const int AliasPlayer = 0;
    public const int AliasTravel = 1;
    public const int AliasCamp = 2;
    public const int AliasHQ = 0;
    public const int FollowerAliases = 5;
    public const int StashCount = 8;

    public Builder(SkyrimMod mod, FormIdRegistry ids)
    {
        _mod = mod;
        _ids = ids;
    }

    private FormKey Id(string editorId) => _ids.Get(editorId);
    private static FormLink<T> L<T>(FormKey k) where T : class, IMajorRecordGetter => new(k);

    public void Build()
    {
        var qMain = Id("CQM_QuartermasterQuest");
        var qFollow = Id("CQM_FollowerQuest");
        var qHQ = Id("CQM_HQQuest");

        var gGranted = Global("CQM_PowerGranted", 0);
        var gHire = Global("CQM_HireCost", 500);
        var gRadius = Global("CQM_ServiceRadius", 1500);
        var gSell = Global("CQM_SellRate", 0.6f);
        var gDays = Global("CQM_RepairDays", 3);
        var gRepairGold = Global("CQM_RepairGold", 50);
        var gCatchup = Global("CQM_CatchupDistance", 3000);
        var gEmber = Global("CQM_EmberPresent", 0);
        var gHired = Global("CQM_Hired", 0);
        var gTraveling = Global("CQM_Traveling", 0);
        var gHQSet = Global("CQM_HQSet", 0);
        var gHQMode = Global("CQM_HQMode", 0);

        var tiers = new (string Edid, int Value, FormKey[] Items)[]
        {
            ("CQM_JunkTier1", 1, new[] { Tankard, WoodenPlate, WoodenBowl, Ladle, Basket01, Basket02, Basket03, Basket04, Basket05, BasketClosed, Bucket, Broom }),
            ("CQM_JunkTier3", 3, new[] { Kettle, Spigot, Fork, Knife, Quill, Inkwell, Flagon, Plate01 }),
            ("CQM_JunkTier6", 6, new[] { Plate02, PotSmall, PotMedium, Lantern, GlazedBowl, GlazedGoblet, GlazedCup, GlazedPlate }),
            ("CQM_JunkTier12", 12, new[] { SilverGoblet01, SilverGoblet02, SilverPlate, SilverBowl01, SilverBowl02, SilverPlatter, SilverJug, DwarvenBowl }),
            ("CQM_JunkTier25", 25, new[] { SilverCandle01, SilverCandle02, SilverCandle03, DwarvenHighPlate, DwarvenHighCup, DwarvenHighBowl, DwarvenBowl02, DwarvenPot, DwarvenUrn }),
        };
        var junkLists = new FormKey[tiers.Length];
        var tierValues = new int[tiers.Length];
        for (int i = 0; i < tiers.Length; i++)
        {
            junkLists[i] = FormList(tiers[i].Edid, tiers[i].Items);
            tierValues[i] = tiers[i].Value;
        }

        var stock = new (FormKey Item, int Count)[]
        {
            (FoodBread01A, 10), (FoodCheeseWedge01, 8), (FoodVenison, 8),
            (CureDisease, 4), (RestoreHealth01, 6), (RestoreHealth02, 4),
            (RestoreStamina01, 6), (RestoreStamina02, 4),
            (IronArrow, 40), (SteelArrow, 24),
            (LeatherStrips, 12), (IngotIron, 8), (Charcoal, 10), (Leather01, 6),
        };
        var buy = FormList("CQM_BuyList", stock.Select(s => s.Item).ToArray());
        var merchantBase = Chest("CQM_MerchantChest", "Company Stores", respawn: true, stock);
        var stashBase = Chest("CQM_StashChest", "Company Stash", respawn: false, Array.Empty<(FormKey, int)>());

        var cell = AddInterior("CQM_StorageCell", "Company Stores");
        var merchantRef = Place(cell, "CQM_MerchantChestRef", merchantBase, 0, 0, 0);
        var stashes = new FormKey[StashCount];
        for (int i = 0; i < StashCount; i++)
            stashes[i] = Place(cell, $"CQM_StashRef{i + 1:00}", stashBase, 160f * i, 200, 0);
        var campMarker = Place(cell, "CQM_CampMarkerRef", XMarker, 0, 400, 0);
        var hqMarker = Place(cell, "CQM_HQMarkerRef", XMarker, 400, 400, 0);

        var faction = Add(new Faction(Id("CQM_VendorFaction"), R)
        {
            EditorID = "CQM_VendorFaction",
            Name = "Company Quartermaster",
            Flags = Faction.FactionFlag.Vendor,
            VendorBuySellList = new FormLinkNullable<IFormListGetter>(buy),
            MerchantContainer = new FormLinkNullable<IPlacedObjectGetter>(merchantRef),
            VendorValues = new VendorValues
            {
                StartHour = 0,
                EndHour = 24,
                Radius = 50000,
                NotSellBuy = false,
                OnlyBuysStolenItems = false,
            },
        });

        var npc = new Npc(Id("CQM_Brynja"), R)
        {
            EditorID = "CQM_Brynja",
            Name = "Brynja Tally-Hand",
            ShortName = "Brynja",
            ObjectBounds = Bounds(-22, -14, 0, 22, 14, 128),
            Race = L<IRaceGetter>(NordRace),
            Class = L<IClassGetter>(ClassWarrior1H),
            CombatStyle = L<ICombatStyleGetter>(CsHumanMelee).AsNullable(),
            Configuration = new NpcConfiguration
            {
                Flags = NpcConfiguration.Flag.AutoCalcStats | NpcConfiguration.Flag.Unique | NpcConfiguration.Flag.Protected | NpcConfiguration.Flag.Female,
                Level = new PcLevelMult { LevelMult = 1 },
                CalcMinLevel = 1,
                CalcMaxLevel = 0,
                SpeedMultiplier = 100,
                DispositionBase = 35,
            },
            AIData = new AIData
            {
                Aggression = Aggression.Unaggressive, Confidence = Confidence.Brave, EnergyLevel = 50,
                Responsibility = Responsibility.NoCrime, Mood = Mood.Neutral, Assistance = Assistance.HelpsNobody,
            },
            PlayerSkills = new PlayerSkills(),
            Height = 1,
            Weight = 50,
        };
        npc.HeadParts.Add(L<IHeadPartGetter>(FemaleHeadNord));
        npc.HeadParts.Add(L<IHeadPartGetter>(HairFemaleNord02));
        npc.HeadParts.Add(L<IHeadPartGetter>(HairLineFemaleNord02));
        npc.HeadParts.Add(L<IHeadPartGetter>(FemaleEyesHumanBrown));
        npc.HeadParts.Add(L<IHeadPartGetter>(FemaleBrowsHuman03));
        npc.DefaultOutfit = L<IOutfitGetter>(ArmorIronandHideMixed).AsNullable();
        npc.Items = new ExtendedList<ContainerEntry>
        {
            new ContainerEntry
            {
                Item = new ContainerItem { Item = L<IItemGetter>(IronSword), Count = 1 },
            },
        };
        npc.Factions.Add(new RankPlacement { Faction = L<IFactionGetter>(faction.FormKey), Rank = 0 });
        Add(npc);

        var effArmor = Effect("CQM_EffRepairedArmor", "Field Refit", ActorValue.DamageResist, "Armor rating is increased by <mag>.");
        var effDmg = Effect("CQM_EffRepairedDamage", "Field Refit", ActorValue.AttackDamageMult, "Weapon attacks do <mag>% more damage.");
        var effEmberArmor = Effect("CQM_EffEmberArmor", "Ember Refit", ActorValue.DamageResist, "Armor rating is increased by <mag>.");
        var effEmberDmg = Effect("CQM_EffEmberDamage", "Ember Refit", ActorValue.AttackDamageMult, "Weapon attacks do <mag>% more damage.");
        var abRepair = Ability("CQM_AbRepaired", "Field Refit", "A field refit: +40 armor, weapon attacks do 10% more damage.",
            (effArmor, 40f), (effDmg, 0.10f));
        var abEmber = Ability("CQM_AbEmberRepaired", "Ember Refit", "An ember refit: +60 armor, weapon attacks do 15% more damage.",
            (effEmberArmor, 60f), (effEmberDmg, 0.15f));

        var mgefPower = Add(new MagicEffect(Id("CQM_PowerEffect"), R)
        {
            EditorID = "CQM_PowerEffect",
            Name = "Company Quartermaster",
            Flags = MagicEffect.Flag.NoDuration | MagicEffect.Flag.NoMagnitude | MagicEffect.Flag.NoArea
                    | MagicEffect.Flag.Painless | MagicEffect.Flag.NoHitEvent | MagicEffect.Flag.HideInUI,
            MagicSkill = ActorValue.None, ResistValue = ActorValue.None, SecondActorValue = ActorValue.None,
            Archetype = new MagicEffectArchetype { Type = MagicEffectArchetype.TypeEnum.Script, ActorValue = ActorValue.None },
            CastType = CastType.FireAndForget,
            TargetType = TargetType.Self,
            CastingSoundLevel = SoundLevel.Normal,
            Description = "Speak with the company quartermaster.",
            VirtualMachineAdapter = Vmad.Adapter(Vmad.Script("CQM_PowerEffectScript", Vmad.Obj("Quartermaster", qMain))),
        });
        var power = Add(new Spell(Id("CQM_PowerQuartermaster"), R)
        {
            EditorID = "CQM_PowerQuartermaster",
            Name = "Company Quartermaster",
            Description = "Hire Brynja Tally-Hand at an inn, or speak with her once she is in your service.",
            Type = SpellType.LesserPower,
            CastType = CastType.FireAndForget,
            TargetType = TargetType.Self,
            Flags = SpellDataFlag.ManualCostCalc | SpellDataFlag.NoAbsorbOrReflect,
            EquipmentType = L<IEquipTypeGetter>(EquipVoice).AsNullable(),
            Effects = { Fx(mgefPower.FormKey, 0, 0) },
        });

        var msgIntro = Msg("CQM_MsgIntro", Texts.Intro, Btn("Ok"));
        var msgNotInn = Notice("CQM_MsgNotInn", Texts.NotInn);
        var msgHire = Msg("CQM_MsgHire", Texts.Hire, Btn("Hire"), Btn("Cancel"));
        var msgNoGold = Notice("CQM_MsgNoGold", Texts.NoGold);
        var hiredButtonsTravel = new[] { Btn("Trade supplies"), Btn("Sell all junk"), Btn("Follower services"), Btn("Make camp here"), Btn("Company HQ"), Btn("Dismiss"), Btn("Cancel") };
        var hiredButtonsCamp = new[] { Btn("Trade supplies"), Btn("Sell all junk"), Btn("Follower services"), Btn("Travel with the company"), Btn("Company HQ"), Btn("Dismiss"), Btn("Cancel") };
        var hiredButtonsHQ = new[] { Btn("Trade supplies"), Btn("Sell all junk"), Btn("Follower services"), Btn("Travel with the company"), Btn("Company HQ"), Btn("Dismiss"), Btn("Cancel") };
        var msgTravel = Msg("CQM_MsgHiredTravel", Texts.HiredTravel, hiredButtonsTravel);
        var msgCamp = Msg("CQM_MsgHiredCamp", Texts.HiredCamp, hiredButtonsCamp);
        var msgHiredHQ = Msg("CQM_MsgHiredHQ", Texts.HiredHQ, hiredButtonsHQ);
        var msgTooFar = Notice("CQM_MsgTooFar", Texts.TooFar);
        var msgSell = Msg("CQM_MsgSellConfirm", Texts.SellConfirm, Btn("Sell"), Btn("Cancel"));
        var msgSellNone = Notice("CQM_MsgSellNone", Texts.SellNone);
        var msgFollowers = Msg("CQM_MsgFollowers", Texts.Followers, qFollow,
            Btn("Follower 1"), Btn("Follower 2"), Btn("Follower 3"), Btn("Follower 4"), Btn("Follower 5"), Btn("Cancel"));
        var msgNobody = Notice("CQM_MsgNobody", Texts.Nobody);
        var serviceMenus = new FormKey[FollowerAliases];
        for (int i = 0; i < FollowerAliases; i++)
            serviceMenus[i] = Msg($"CQM_MsgService{i + 1}", Texts.Service(i + 1), qFollow, Btn("Repair gear"), Btn("Open stash"), Btn("Cancel"));
        var emberBtn = Btn("Ember repair");
        emberBtn.Conditions.Add(Cond.Global(gEmber, CompareOperator.EqualTo, 1));
        var msgRepair = Msg("CQM_MsgRepair", Texts.Repair, Btn("Repair gear"), emberBtn, Btn("Cancel"));
        var msgRepairMats = Notice("CQM_MsgRepairMats", Texts.RepairMats);
        var msgEmberMats = Notice("CQM_MsgEmberMats", Texts.EmberMats);
        var msgRepairFull = Notice("CQM_MsgRepairFull", Texts.RepairFull);
        var msgStashFull = Notice("CQM_MsgStashFull", Texts.StashFull);
        var msgDismiss = Msg("CQM_MsgDismiss", Texts.Dismiss, Btn("Dismiss"), Btn("Cancel"));
        var msgDismissHQ = Msg("CQM_MsgDismissHQ", Texts.DismissHQ, Btn("Send her to HQ"), Btn("Release her from service"), Btn("Cancel"));
        var sendHQ = Btn("Send her to HQ");
        sendHQ.Conditions.Add(Cond.Global(gHQSet, CompareOperator.EqualTo, 1));
        var clearHQ = Btn("Clear HQ");
        clearHQ.Conditions.Add(Cond.Global(gHQSet, CompareOperator.EqualTo, 1));
        var msgHQ = Msg("CQM_MsgHQ", Texts.HQ, Btn("Set company HQ here"), sendHQ, clearHQ, Btn("Cancel"));
        var msgHQNotHere = Notice("CQM_MsgHQNotHere", Texts.HQNotHere);
        var msgFallen = Notice("CQM_MsgFallen", Texts.Fallen);

        var follow = Add(new Quest(qFollow, R)
        {
            EditorID = "CQM_FollowerQuest",
            Name = "Company Quartermaster: Followers",
            Priority = 30,
            Type = Quest.TypeEnum.None,
            QuestFormVersion = VanillaQuestFormVersion,
        });
        for (int i = 0; i < FollowerAliases; i++)
        {
            var a = new QuestAlias
            {
                ID = (uint)i,
                Type = QuestAlias.TypeEnum.Reference,
                Name = $"Follower{i + 1}",
                Flags = QuestAlias.Flag.Optional | QuestAlias.Flag.MatchingRefInLoadedArea | QuestAlias.Flag.MatchingRefClosest | QuestAlias.Flag.StoresText,
            };
            a.Conditions.Add(Cond.Make(new GetPlayerTeammateConditionData(), 1, CompareOperator.EqualTo));
            a.Conditions.Add(Cond.Make(new GetDeadConditionData(), 0, CompareOperator.EqualTo));
            follow.Aliases.Add(a);
        }
        follow.NextAliasID = FollowerAliases;

        var hq = Add(new Quest(qHQ, R)
        {
            EditorID = "CQM_HQQuest",
            Name = "Company HQ",
            Priority = 30,
            Type = Quest.TypeEnum.None,
            QuestFormVersion = VanillaQuestFormVersion,
        });
        hq.Aliases.Add(AliasWithPackage(AliasHQ, "CQM_HQAlias", DefaultSandboxCurrentLocation1024));
        hq.NextAliasID = 1;
        follow.Stages.Add(new QuestStage
        {
            Index = 10,
            LogEntries = { new QuestLogEntry { Flags = 0, Entry = Texts.FollowerLog } },
        });

        var quest = Add(new Quest(qMain, R)
        {
            EditorID = "CQM_QuartermasterQuest",
            Name = "Company Quartermaster",
            Flags = Quest.Flag.StartGameEnabled | Quest.Flag.RunOnce,
            Priority = 30,
            Type = Quest.TypeEnum.None,
            QuestFormVersion = VanillaQuestFormVersion,
        });
        quest.Aliases.Add(new QuestAlias
        {
            ID = AliasPlayer, Type = QuestAlias.TypeEnum.Reference, Name = "Player",
            ForcedReference = new FormLinkNullable<IPlacedGetter>(PlayerRef),
        });
        quest.Aliases.Add(AliasWithPackage(AliasTravel, "CQM_TravelAlias", DefaultFollowPlayer));
        quest.Aliases.Add(AliasWithPackage(AliasCamp, "CQM_CampAlias", DefaultSandboxCurrentLocation256));
        quest.NextAliasID = 3;
        quest.VirtualMachineAdapter = Vmad.QuestAdapter(Vmad.Script("CQM_QuartermasterScript",
                Vmad.Obj("CQM_PowerGranted", gGranted), Vmad.Obj("CQM_HireCost", gHire), Vmad.Obj("CQM_ServiceRadius", gRadius),
                Vmad.Obj("CQM_SellRate", gSell), Vmad.Obj("CQM_RepairDays", gDays), Vmad.Obj("CQM_RepairGold", gRepairGold),
                Vmad.Obj("CQM_CatchupDistance", gCatchup), Vmad.Obj("CQM_EmberPresent", gEmber),
                Vmad.Obj("CQM_Hired", gHired), Vmad.Obj("CQM_Traveling", gTraveling),
                Vmad.Obj("CQM_HQSet", gHQSet), Vmad.Obj("CQM_HQMode", gHQMode),
                Vmad.Obj("CQM_Brynja", npc.FormKey), Vmad.Obj("CQM_PowerQuartermaster", power.FormKey),
                Vmad.Obj("CQM_AbRepaired", abRepair), Vmad.Obj("CQM_AbEmberRepaired", abEmber),
                Vmad.Obj("LocTypeInn", LocTypeInn), Vmad.Obj("LocTypePlayerHouse", LocTypePlayerHouse),
                Vmad.Obj("Gold001", Gold001), Vmad.Obj("LeatherStrips", LeatherStrips), Vmad.Obj("IngotIron", IngotIron),
                Vmad.Obj("Charcoal", Charcoal), Vmad.Obj("FireSalts", FireSalts),
                Vmad.ObjList("JunkLists", junkLists), Vmad.IntList("TierValues", tierValues),
                Vmad.ObjList("Stashes", stashes), Vmad.Obj("CQM_CampMarker", campMarker), Vmad.Obj("CQM_HQMarker", hqMarker),
                Vmad.Alias("CQM_TravelAlias", qMain, AliasTravel), Vmad.Alias("CQM_CampAlias", qMain, AliasCamp),
                Vmad.Alias("CQM_HQAlias", qHQ, AliasHQ), Vmad.Obj("CQM_HQQuest", qHQ),
                Vmad.Obj("CQM_FollowerQuest", qFollow),
                Vmad.AliasList("Followers", qFollow, Enumerable.Range(0, FollowerAliases).ToArray()),
                Vmad.Obj("CQM_MsgIntro", msgIntro), Vmad.Obj("CQM_MsgNotInn", msgNotInn), Vmad.Obj("CQM_MsgHire", msgHire),
                Vmad.Obj("CQM_MsgNoGold", msgNoGold), Vmad.Obj("CQM_MsgHiredTravel", msgTravel), Vmad.Obj("CQM_MsgHiredCamp", msgCamp),
                Vmad.Obj("CQM_MsgHiredHQ", msgHiredHQ),
                Vmad.Obj("CQM_MsgTooFar", msgTooFar), Vmad.Obj("CQM_MsgSellConfirm", msgSell), Vmad.Obj("CQM_MsgSellNone", msgSellNone),
                Vmad.Obj("CQM_MsgFollowers", msgFollowers), Vmad.Obj("CQM_MsgNobody", msgNobody),
                Vmad.ObjList("ServiceMenus", serviceMenus),
                Vmad.Obj("CQM_MsgRepair", msgRepair), Vmad.Obj("CQM_MsgRepairMats", msgRepairMats), Vmad.Obj("CQM_MsgEmberMats", msgEmberMats),
                Vmad.Obj("CQM_MsgRepairFull", msgRepairFull), Vmad.Obj("CQM_MsgStashFull", msgStashFull),
                Vmad.Obj("CQM_MsgDismiss", msgDismiss), Vmad.Obj("CQM_MsgDismissHQ", msgDismissHQ),
                Vmad.Obj("CQM_MsgHQ", msgHQ), Vmad.Obj("CQM_MsgHQNotHere", msgHQNotHere),
                Vmad.Obj("CQM_MsgFallen", msgFallen)))
            .WithAliasScripts(qMain, AliasPlayer, Vmad.Script("CQM_PlayerAliasScript", Vmad.Obj("Quartermaster", qMain)));
    }

    private QuestAlias AliasWithPackage(int id, string name, FormKey package)
    {
        var a = new QuestAlias
        {
            ID = (uint)id,
            Type = QuestAlias.TypeEnum.Reference,
            Name = name,
            Flags = QuestAlias.Flag.Optional | QuestAlias.Flag.Protected,
        };
        a.PackageData.Add(new FormLink<IPackageGetter>(package));
        return a;
    }

    private Cell AddInterior(string edid, string name)
    {
        var cell = new Cell(Id(edid), R)
        {
            EditorID = edid,
            Name = name,
            Flags = Cell.Flag.IsInteriorCell | Cell.Flag.CantTravelFromHere,
        };
        _mod.Cells.AddInteriorCell(cell);
        return cell;
    }

    private FormKey Place(Cell cell, string edid, FormKey baseForm, float x, float y, float z)
    {
        var p = new PlacedObject(Id(edid), R)
        {
            EditorID = edid,
            Base = new FormLinkNullable<IPlaceableObjectGetter>(baseForm),
            Placement = new Placement
            {
                Position = new P3Float(x, y, z),
                Rotation = new P3Float(0, 0, 0),
            },
            MajorRecordFlagsRaw = (int)PlacedObject.ContainerMajorFlag.Persistent,
        };
        cell.Persistent.Add(p);
        return p.FormKey;
    }

    private FormKey Chest(string edid, string name, bool respawn, (FormKey Item, int Count)[] stock)
    {
        var c = new Container(Id(edid), R)
        {
            EditorID = edid,
            Name = name,
            ObjectBounds = Bounds(-32, -20, 0, 32, 20, 48),
            Model = new Model { File = MeshChest },
            Weight = 0,
            Flags = respawn ? Container.Flag.Respawns : 0,
        };
        if (stock.Length > 0)
        {
            c.Items = stock.Select(s => new ContainerEntry
            {
                Item = new ContainerItem { Item = L<IItemGetter>(s.Item), Count = s.Count },
            }).ToExtendedList();
        }
        Add(c);
        return c.FormKey;
    }

    private FormKey FormList(string edid, FormKey[] items)
    {
        var list = new FormList(Id(edid), R) { EditorID = edid };
        foreach (var item in items) list.Items.Add(L<ISkyrimMajorRecordGetter>(item));
        Add(list);
        return list.FormKey;
    }

    private FormKey Ability(string edid, string name, string description, params (FormKey Effect, float Magnitude)[] effects)
    {
        var spell = Add(new Spell(Id(edid), R)
        {
            EditorID = edid,
            Name = name,
            Description = description,
            Type = SpellType.Ability,
            CastType = CastType.ConstantEffect,
            TargetType = TargetType.Self,
            Flags = SpellDataFlag.ManualCostCalc | SpellDataFlag.NoAbsorbOrReflect,
            Effects = effects.Select(e => Fx(e.Effect, e.Magnitude, 0)).ToExtendedList(),
        });
        return spell.FormKey;
    }

    private T Add<T>(T record) where T : SkyrimMajorRecord
    {
        _mod.GetTopLevelGroup<T>().Add(record);
        return record;
    }

    private FormKey Global(string edid, float value)
    {
        var g = new GlobalFloat(Id(edid), R) { EditorID = edid, Data = value };
        _mod.Globals.Add(g);
        return g.FormKey;
    }

    private FormKey Effect(string edid, string name, ActorValue av, string description)
    {
        var m = new MagicEffect(Id(edid), R)
        {
            EditorID = edid,
            Name = name,
            Flags = MagicEffect.Flag.Recover | MagicEffect.Flag.NoDuration | MagicEffect.Flag.NoArea | MagicEffect.Flag.NoHitEvent,
            MagicSkill = ActorValue.None, ResistValue = ActorValue.None, SecondActorValue = ActorValue.None,
            Archetype = new MagicEffectArchetype { Type = MagicEffectArchetype.TypeEnum.ValueModifier, ActorValue = av },
            CastType = CastType.ConstantEffect,
            TargetType = TargetType.Self,
            CastingSoundLevel = SoundLevel.Normal,
            Description = description,
        };
        _mod.MagicEffects.Add(m);
        return m.FormKey;
    }

    private static Effect Fx(FormKey mgef, float magnitude, int duration) => new()
    {
        BaseEffect = new FormLinkNullable<IMagicEffectGetter>(mgef),
        Data = new EffectData { Magnitude = magnitude, Area = 0, Duration = duration },
    };

    private static MessageButton Btn(string text) => new() { Text = text };

    private FormKey Msg(string edid, string text, params MessageButton[] buttons) => Msg(edid, text, null, buttons);

    private FormKey Msg(string edid, string text, FormKey? owner, params MessageButton[] buttons)
    {
        var m = new Message(Id(edid), R)
        {
            EditorID = edid,
            Description = text,
            Flags = Message.Flag.MessageBox,
            INAM = new byte[4],
        };
        if (owner is FormKey k) m.Quest.SetTo(k);
        m.MenuButtons.AddRange(buttons);
        _mod.Messages.Add(m);
        return m.FormKey;
    }

    private FormKey Notice(string edid, string text)
    {
        var m = new Message(Id(edid), R) { EditorID = edid, Description = text, Flags = 0, DisplayTime = 2, INAM = new byte[4] };
        _mod.Messages.Add(m);
        return m.FormKey;
    }

    private static ObjectBounds Bounds(short x1, short y1, short z1, short x2, short y2, short z2)
        => new() { First = new P3Int16(x1, y1, z1), Second = new P3Int16(x2, y2, z2) };
}
