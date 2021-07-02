using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Threading.Tasks;

namespace yoo_math_bot
{
    public class Commands : ModuleBase
    {
        public static readonly string logo = Program.logo;

        [Command("invite")]
        public async Task InviteCommand()
        {
            var embed = new EmbedBuilder();
            embed.WithTitle("Invite Yoo-Math to Your Server!");
            embed.WithUrl("https://www.math.yoo-babobo.com/bot");
            embed.WithColor(Color.Gold);
            embed.WithThumbnailUrl(logo);
            embed.WithDescription("[Click Here](https://www.math.yoo-babobo.com/bot 'This is the link to invite me.') You won't regret it!");
            embed.WithImageUrl(logo);
            await ReplyAsync(null, false, embed.Build());
        }

        [Command("dev")]
        public async Task DevCommand()
        {
            var embed = new EmbedBuilder();
            embed.WithTitle("The Development of Yoo-Math");
            embed.WithUrl("https://github.com/Yoo-Babobo/Yoo-Math_Discord_Bot");
            embed.WithColor(Color.Gold);
            embed.WithThumbnailUrl(logo);
            embed.WithDescription("My GitHub is [here](https://github.com/Yoo-Babobo/Yoo-Math_Discord_Bot) and the Yoo-Math app's GitHub is [here](https://github.com/Yoo-Babobo/Yoo-Math).\n\nI was made by [**Yoo-Babobo**](https://www.yoo-babobo.com/dev).");
            embed.WithImageUrl(logo);
            await ReplyAsync(null, false, embed.Build());
        }

        [Command("about")]
        public async Task AboutCommand()
        {
            var embed = new EmbedBuilder();
            embed.WithTitle("About Yoo-math");
            embed.WithUrl("https://www.math.yoo-babobo.com");
            embed.WithColor(Color.Gold);
            embed.WithThumbnailUrl(logo);
            embed.WithDescription("```\"The future of math.\"```– *Yoo-Babobo*");
            embed.WithImageUrl(logo);
            await ReplyAsync(null, false, embed.Build());
        }

        [Command("help")]
        [Alias("?")]
        public async Task HelpCommand()
        {
            string commands = new WebClient().DownloadString("https://www.data.yoo-babobo.com/data/discord_bots/YooMath/commands.yoo");
            var help_commands = new List<string>();
            foreach (var command in commands.Split("\n"))
            {
                string[] split_command = command.ToString().Split(',');
                help_commands.Add(split_command[0] + " — " + split_command[1]);
            }
            var embed = new EmbedBuilder();
            embed.WithTitle("Yoo-Math Help");
            embed.WithColor(Color.Gold);
            embed.WithThumbnailUrl(logo);
            embed.WithDescription("My prefix is **^**\n\nMy commands:```" + string.Join("``````", help_commands) + "```\nMy website:\nhttps://www.math.yoo-babobo.com\n\nI was made by [**Yoo-Babobo**](https://www.yoo-babobo.com/dev 'Their Website').");
            embed.WithImageUrl(logo);
            await ReplyAsync(null, false, embed.Build());
        }

        [Command("change-log")]
        [Alias("cl")]
        public async Task ChangeLogCommand()
        {
            string change_log = new WebClient().DownloadString("https://www.data.yoo-babobo.com/data/discord_bots/YooMath/change_log.yoo");
            var new_change_log = new List<string>();
            var new_new_change_log = new List<string>();
            foreach (var log in change_log.Split("\n"))
            {
                string[] split_log = log.ToString().Split(',');
                new_change_log.Add("*" + split_log[0] + "* **—** `" + split_log[1] + "`");
            }
            new_new_change_log.Add(new_change_log[0]);
            new_new_change_log.Add(new_change_log[1]);
            new_new_change_log.Add(new_change_log[2]);
            var embed = new EmbedBuilder();
            embed.WithTitle("Yoo-Math Change Logs");
            embed.WithColor(Color.Gold);
            embed.WithThumbnailUrl(logo);
            embed.WithDescription(string.Join("\n", new_new_change_log));
            embed.WithImageUrl(logo);
            await ReplyAsync(null, false, embed.Build());
        }

        [Command("calculate")]
        [Alias("cal", "math")]
        public async Task CalculateCommand([Remainder] string calculation = null)
        {
            if (string.IsNullOrEmpty(calculation)) await ReplyAsync("Please include math!"); else await ReplyAsync(Eval(calculation));
        }

        [Command("website")]
        [Alias("web")]
        public async Task WebsiteCommand() { await ReplyAsync("https://www.math.yoo-babobo.com"); }

        [Command("int")]
        public async Task IntCommand() { await ReplyAsync(new Random().Next(int.MinValue, int.MaxValue).ToString()); }

        [Command("double")]
        public async Task DoubleCommand() { await ReplyAsync((new Random().Next(int.MinValue, int.MaxValue) + new Random().NextDouble()).ToString()); }

        public static string Eval(string expression)
        {
            try
            {
                expression = expression.ToLower().Replace('x', '*').Replace("%", "/100").Replace("p", Math.PI.ToString()).Replace("e", Math.E.ToString());
                DataTable table = new DataTable();
                var opt = Convert.ToDouble(table.Compute(expression, string.Empty)).ToString();
                return opt;
            }
            catch { return double.NaN.ToString(); }
        }
    }
}