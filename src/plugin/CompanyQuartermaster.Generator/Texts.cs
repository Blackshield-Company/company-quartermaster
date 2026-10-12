namespace CompanyQuartermaster.Generator;

internal static class Texts
{
    public const string Intro =
        "COMPANY QUARTERMASTER\n\n" +
        "You can hire Brynja Tally-Hand. She keeps stores for the company: supplies, a bulk price on clutter, and a stash and a field refit for each follower.\n\n" +
        "The Blackshield Company has no hall of its own, so she takes hire at an inn. Use the lesser power Company Quartermaster. She is not a follower and does not take a follower slot.";

    public const string NotInn =
        "The company has no hall. Hire Brynja Tally-Hand at an inn.";

    public const string Hire =
        "COMPANY QUARTERMASTER\n\n" +
        "Brynja Tally-Hand will keep the company's stores for %.0f gold.\n\n" +
        "She sells supplies, buys clutter in bulk, and looks after your followers' gear and stashes. She can travel with you or wait in camp. She does not take a follower slot.";

    public const string NoGold = "You need %.0f gold to hire her.";

    public const string HiredTravel =
        "COMPANY QUARTERMASTER\n\nBrynja Tally-Hand is travelling with the company.";

    public const string HiredCamp =
        "COMPANY QUARTERMASTER\n\nBrynja Tally-Hand is camped. She will wait where you left her.";

    public const string TooFar = "Brynja is too far away.";

    public const string SellConfirm = "Sell the clutter for %.0f gold?";
    public const string SellNone = "You have no clutter Brynja will buy.";

    public const string Followers =
        "FOLLOWER SERVICES\n\nWho needs the quartermaster?\n\n<Alias=Follower1>\n<Alias=Follower2>\n<Alias=Follower3>\n<Alias=Follower4>\n<Alias=Follower5>";

    public const string Nobody = "That companion isn't here.";

    public static string Service(int n) =>
        $"FOLLOWER SERVICES\n\n<Alias=Follower{n}>";

    public const string Repair =
        "FIELD REFIT\n\nA field refit holds for %.0f days: +40 armor, weapon attacks do 10% more damage. It costs 2 leather strips, 1 iron ingot and %.0f gold.\n\nEmber repair, when the rite is at hand, holds harder (+60 armor, 15% more damage) and costs 2 charcoal and 1 fire salts.";

    public const string RepairMats = "A refit takes 2 leather strips, 1 iron ingot and %.0f gold.";
    public const string EmberMats = "An ember refit takes 2 charcoal and 1 fire salts.";
    public const string RepairFull = "Too many refits are out at once. Wait for one to wear off.";

    public const string StashFull = "All eight stash slots are in use.";

    public const string Dismiss =
        "Send Brynja away? Her stashes keep what you stored. Hire her again at an inn.";

    public const string Fallen = "Brynja has fallen. Hire her again at an inn when you can.";

    public const string FollowerLog =
        "Companions: <Alias=Follower1> <Alias=Follower2> <Alias=Follower3> <Alias=Follower4> <Alias=Follower5>";
}
