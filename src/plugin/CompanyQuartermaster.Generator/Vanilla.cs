using Mutagen.Bethesda.Plugins;

namespace CompanyQuartermaster.Generator;

/// <summary>
/// Skyrim.esm records this mod links to. FormIDs were looked up with the
/// Mutagen.Bethesda.FormKeys.SkyrimSE helper in _ref/fk (dotnet run). Nothing
/// here is overridden; the mod only references these.
/// </summary>
internal static class Vanilla
{
    public static readonly ModKey Skyrim = ModKey.FromFileName("Skyrim.esm");
    private static FormKey K(uint id) => new(Skyrim, id);

    public static readonly FormKey PlayerRef = K(0x000014);
    public static readonly FormKey EquipVoice = K(0x025BEE);
    public static readonly FormKey Gold001 = K(0x00000F);
    public static readonly FormKey XMarker = K(0x00003B);

    public static readonly FormKey LocTypeInn = K(0x01CB87);
    public static readonly FormKey LocTypePlayerHouse = K(0x0FC1A3);
    public static readonly FormKey CurrentFollowerFaction = K(0x05C84E);
    public static readonly FormKey PotentialFollowerFaction = K(0x05C84D);

    public static readonly FormKey DefaultFollowPlayer = K(0x04B037);
    public static readonly FormKey DefaultSandboxCurrentLocation256 = K(0x0956B8);
    public static readonly FormKey DefaultSandboxCurrentLocation1024 = K(0x0BFB6B);

    public static readonly FormKey FemaleHeadNord = K(0x051623);
    public static readonly FormKey HairFemaleNord02 = K(0x051193);
    public static readonly FormKey HairLineFemaleNord02 = K(0x0510F6);
    public static readonly FormKey FemaleEyesHumanBrown = K(0x072917);
    public static readonly FormKey FemaleBrowsHuman03 = K(0x0E4D91);
    public static readonly FormKey ArmorIronandHideMixed = K(0x0ABDA1);
    public static readonly FormKey IronSword = K(0x012EB7);
    public static readonly FormKey UthgerdNpc = K(0x0918E2); // Skyrim+Npc.Uthgerd
    public static readonly FormKey NordRace = K(0x013746);
    public static readonly FormKey ClassWarrior1H = K(0x013176);
    public static readonly FormKey CsHumanMelee = K(0x03BE1B);

    public static readonly FormKey FoodBread01A = K(0x065C97);
    public static readonly FormKey FoodCheeseWedge01 = K(0x064B31);
    public static readonly FormKey FoodVenison = K(0x0669A2);
    public static readonly FormKey CureDisease = K(0x0AE723);
    public static readonly FormKey RestoreHealth01 = K(0x03EADD);
    public static readonly FormKey RestoreHealth02 = K(0x03EADE);
    public static readonly FormKey RestoreStamina01 = K(0x03EAE5);
    public static readonly FormKey RestoreStamina02 = K(0x039BE8);
    public static readonly FormKey IronArrow = K(0x01397D);
    public static readonly FormKey SteelArrow = K(0x01397F);
    public static readonly FormKey LeatherStrips = K(0x0800E4);
    public static readonly FormKey IngotIron = K(0x05ACE4);
    public static readonly FormKey Charcoal = K(0x033760);
    public static readonly FormKey Leather01 = K(0x0DB5D2);
    public static readonly FormKey FireSalts = K(0x03AD5E);

    // Junk clutter, grouped by the bulk tier we pay. Not quest items, gems, or ingots.
    public static readonly FormKey Tankard = K(0x0319E3);
    public static readonly FormKey WoodenPlate = K(0x031941);
    public static readonly FormKey WoodenBowl = K(0x03199A);
    public static readonly FormKey Ladle = K(0x0319E5);
    public static readonly FormKey Basket01 = K(0x012FE7);
    public static readonly FormKey Basket02 = K(0x012FE8);
    public static readonly FormKey Basket03 = K(0x012FE9);
    public static readonly FormKey Basket04 = K(0x012FEA);
    public static readonly FormKey Basket05 = K(0x012FEB);
    public static readonly FormKey BasketClosed = K(0x012FEC);
    public static readonly FormKey Bucket = K(0x012FDF);
    public static readonly FormKey Broom = K(0x06717F);

    public static readonly FormKey Kettle = K(0x012FE6);
    public static readonly FormKey Spigot = K(0x03807B);
    public static readonly FormKey Fork = K(0x104B3F);
    public static readonly FormKey Knife = K(0x104B40);
    public static readonly FormKey Quill = K(0x04C3C8);
    public static readonly FormKey Inkwell = K(0x04C3C6);
    public static readonly FormKey Flagon = K(0x044E70);
    public static readonly FormKey Plate01 = K(0x0E2617);

    public static readonly FormKey Plate02 = K(0x0E2618);
    public static readonly FormKey PotSmall = K(0x0318FA);
    public static readonly FormKey PotMedium = K(0x0318FB);
    public static readonly FormKey Lantern = K(0x0318EC);
    public static readonly FormKey GlazedBowl = K(0x0B9BCC);
    public static readonly FormKey GlazedGoblet = K(0x0B9BDA);
    public static readonly FormKey GlazedCup = K(0x0B9BDC);
    public static readonly FormKey GlazedPlate = K(0x0B9BD0);

    public static readonly FormKey GlazedBowlNordic = K(0x0F08FA);
    public static readonly FormKey GlazedGobletNordic = K(0x0F08F9);
    public static readonly FormKey GlazedCupNordic = K(0x0F08F8);
    public static readonly FormKey GlazedPlateNordic = K(0x0F08F5);
    public static readonly FormKey GlazedBowl02Nordic = K(0x0F08FB);
    public static readonly FormKey GlazedPotNordic = K(0x0F08F6);
    public static readonly FormKey GlazedPot02Nordic = K(0x0F08F7);
    public static readonly FormKey GlazedJugNordic = K(0x0F08F3);

    public static readonly FormKey SilverGoblet01 = K(0x098620);
    public static readonly FormKey SilverGoblet02 = K(0x098621);
    public static readonly FormKey SilverPlate = K(0x098624);
    public static readonly FormKey SilverBowl01 = K(0x098626);
    public static readonly FormKey SilverBowl02 = K(0x098627);
    public static readonly FormKey SilverPlatter = K(0x098625);
    public static readonly FormKey SilverJug = K(0x098623);
    public static readonly FormKey SilverCandle01 = K(0x0E42DF);
    public static readonly FormKey SilverCandle02 = K(0x0E42E0);
    public static readonly FormKey SilverCandle03 = K(0x0E42E1);
    public static readonly FormKey DwarvenHighPlate = K(0x027F15);
    public static readonly FormKey DwarvenHighCup = K(0x027F07);
    public static readonly FormKey DwarvenHighBowl = K(0x027F03);
    public static readonly FormKey DwarvenBowl = K(0x0E1FC3);
    public static readonly FormKey DwarvenBowl02 = K(0x025C13);
    public static readonly FormKey DwarvenPot = K(0x025CFE);
    public static readonly FormKey DwarvenUrn = K(0x025D7C);

    public const string MeshChest = @"Clutter\Containers\Chest01.nif";
}
