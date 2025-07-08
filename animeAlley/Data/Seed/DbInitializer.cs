using animeAlley.Data;
using animeAlley.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace animeAlley.Data.Seed
{
    internal class DbInitializer
    {

        internal static async Task Initialize(ApplicationDbContext dbContext)
        {

            /*
             * https://stackoverflow.com/questions/70581816/how-to-seed-data-in-net-core-6-with-entity-framework
             * 
             * https://learn.microsoft.com/en-us/aspnet/core/data/ef-mvc/intro?view=aspnetcore-6.0#initialize-db-with-test-data
             * https://github.com/dotnet/AspNetCore.Docs/blob/main/aspnetcore/data/ef-mvc/intro/samples/5cu/Program.cs
             * https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/style-rules/ide0300
             */

            ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));
            dbContext.Database.EnsureCreated();

            // var auxiliar
            bool haAdicao = false;

            // Se não houver Categorias, cria-as
            var generos = Array.Empty<Genero>();

            if (!dbContext.Generos.Any())
            {
                generos = [
                   new Genero { GeneroNome = "Ação" },
                   new Genero { GeneroNome = "Aventura" },
                   new Genero { GeneroNome = "Comédia" },
                   new Genero { GeneroNome = "Drama" },
                   new Genero { GeneroNome = "Ecchi" },
                   new Genero { GeneroNome = "Fatansia" },
                   new Genero { GeneroNome = "Horror" },
                   new Genero { GeneroNome = "Mahou Shoujo" },
                   new Genero { GeneroNome = "Mecha" },
                   new Genero { GeneroNome = "Música" },
                   new Genero { GeneroNome = "Mistério" },
                   new Genero { GeneroNome = "Psicológico" },
                   new Genero { GeneroNome = "Romance" },
                   new Genero { GeneroNome = "Sci-Fi" },
                   new Genero { GeneroNome = "Slice of Life" },
                   new Genero { GeneroNome = "Esporte" },
                   new Genero { GeneroNome = "Supernatural" },
                   new Genero { GeneroNome = "Thriller" },
                ];

                await dbContext.Generos.AddRangeAsync(generos);
                await dbContext.SaveChangesAsync();
                haAdicao = true;
            }
            else
            {
                generos = await dbContext.Generos.ToArrayAsync();
            }


            // Se não houver Utilizadores Identity, cria-os
            var newIdentityUsers = Array.Empty<IdentityUser>();
            //a hasher to hash the password before seeding the user to the db
            var hasher = new PasswordHasher<IdentityUser>();

            if (await dbContext.Users.CountAsync() == 1)
            {
                newIdentityUsers = [
                   new IdentityUser{
                        UserName="go3370764@gmail.com",
                        NormalizedUserName="GO3370764@GMAIL.COM",
                        Email="go3370764@gmail.com",
                        NormalizedEmail="GO3370764@GMAIL.COM",
                        EmailConfirmed=true,
                        SecurityStamp=Guid.NewGuid().ToString("N").ToUpper(),
                        ConcurrencyStamp=Guid.NewGuid().ToString(),
                        PasswordHash=hasher.HashPassword(null,"12345A@")
                    },
                    new IdentityUser{
                        UserName="kakirmsplatoon2@gmail.com",
                        NormalizedUserName="KAKIRMSPLATOON2@GMAIL.COM",
                        Email="kakirmsplatoon2@gmail.com",
                        NormalizedEmail="KAKIRMSPLATOON2@GMAIL.COM",
                        EmailConfirmed=true,
                        SecurityStamp=Guid.NewGuid().ToString("N").ToUpper(),
                        ConcurrencyStamp=Guid.NewGuid().ToString(),
                        PasswordHash=hasher.HashPassword(null,"ADMIN2025@")
                    }
                ];

                await dbContext.Users.AddRangeAsync(newIdentityUsers);
                await dbContext.SaveChangesAsync();
                haAdicao = true;
            }
            else
            {
                // Get existing users (excluding the admin user we know exists)  
                var existingUsers = await dbContext.Users.Where(u => u.Email != "admin@animealley.com").ToArrayAsync();
                newIdentityUsers = existingUsers;
            }

            var roles = Array.Empty<IdentityUserRole<string>>();

            if (await dbContext.Users.CountAsync() > 1)
            {
                var targetUsers = await dbContext.Users
                    .Where(u => u.Email == "go3370764@gmail.com" || u.Email == "kakirmsplatoon2@gmail.com")
                    .ToArrayAsync();

                var adminRole = await dbContext.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
                var userRole = await dbContext.Roles.FirstOrDefaultAsync(r => r.Name == "User");

                if (targetUsers.Length >= 2 && adminRole != null && userRole != null)
                {
                    var adminUser = targetUsers.First(u => u.Email == "kakirmsplatoon2@gmail.com");
                    var regularUser = targetUsers.First(u => u.Email == "go3370764@gmail.com");

                    var existingAdminRole = await dbContext.UserRoles
                        .FirstOrDefaultAsync(ur => ur.UserId == adminUser.Id && ur.RoleId == adminRole.Id);

                    var existingUserRole = await dbContext.UserRoles
                        .FirstOrDefaultAsync(ur => ur.UserId == regularUser.Id && ur.RoleId == userRole.Id);

                    var rolesToAdd = new List<IdentityUserRole<string>>();

                    if (existingAdminRole == null)
                    {
                        rolesToAdd.Add(new IdentityUserRole<string>
                        {
                            UserId = adminUser.Id,
                            RoleId = adminRole.Id
                        });
                    }

                    if (existingUserRole == null)
                    {
                        rolesToAdd.Add(new IdentityUserRole<string>
                        {
                            UserId = regularUser.Id,
                            RoleId = userRole.Id
                        });
                    }

                    if (rolesToAdd.Any())
                    {
                        await dbContext.UserRoles.AddRangeAsync(rolesToAdd);
                        await dbContext.SaveChangesAsync();
                        haAdicao = true;
                    }
                }
            }


            // NOW create Utilizadores after IdentityUsers exist
            var utilizadores = Array.Empty<Utilizador>();
            if (!await dbContext.Utilizadores.AnyAsync())
            {
                utilizadores = [
                    new Utilizador {
                        Nome="animeAlley",
                        Foto="placeholder.png",
                        UserName=newIdentityUsers[1].UserName,
                        isAdmin=true,
                        Banner="bannerplaceholder.png"
                    },
                    new Utilizador {
                        Nome="Daniel",
                        Foto="placeholder.png",
                        UserName=newIdentityUsers[0].UserName,
                        isAdmin=false,
                        Banner="bannerplaceholder.png"
                    },
                ];

                await dbContext.Utilizadores.AddRangeAsync(utilizadores);
                await dbContext.SaveChangesAsync();
                haAdicao = true;
            }
            else
            {
                utilizadores = await dbContext.Utilizadores.ToArrayAsync();
            }

            // Método para popular dados de personagens baseado na tabela fornecida
            var personagens = Array.Empty<Personagem>();
            if (!dbContext.Personagens.Any())
            {
                personagens = [
                    new Personagem{
                        Nome = "Frieren",
                        TipoPersonagem = TiposPersonagem.Protagonista,
                        Sinopse = "Frieren is the protagonist of Sousou no Frieren and Fern's master. She was the Mage of the Hero Party and traveled alongside Hero Himmel, Warrior Eisen, and Priest Heiter in a 10-year journey to defeat the Demon King.\n\nShe was a magic prodigy at a young age, with an impressive quantity of mana, which caught the interest of Flamme, a human wizard who taught her everything there is to know about magic and mana control until she passed away. Frieren persists in collecting magic knowledge no matter how trivial it may be.",
                        Foto = "db686c87-50ab-4295-97d5-45a86e9dec76.png",
                        DataNasc = null,
                        Idade = 1000,
                        PersonagemSexualidade = Sexualidade.Masculino
                    },
                    new Personagem{
                        Nome = "Fern",
                        TipoPersonagem = TiposPersonagem.Protagonista,
                        Sinopse = "Fern is the young mage apprentice of Frieren and accompanies her as a member of her party. She is an orphaned war refugee originally from the Southern Lands, later adopted by Heiter and placed under Frieren's care.",
                        Foto = "9caf504e-aa81-436d-91b8-d754164c82c3.png",
                        DataNasc = null,
                        Idade = null,
                        PersonagemSexualidade = Sexualidade.Feminino
                    },
                    new Personagem{
                        Nome = "Stark",
                        TipoPersonagem = TiposPersonagem.Protagonista,
                        Sinopse = "Stark is a warrior who fights alongside Frieren and Fern. After his village was attacked by demons, he fled and became Eisen's apprentice. At Eisen's instruction, he joined Frieren's party as their vanguard.",
                        Foto = "48cd1842-c2bd-4c0c-9db8-5b36b4bfdb01.png",
                        DataNasc = null,
                        Idade = 18,
                        PersonagemSexualidade = Sexualidade.Masculino
                    },
                    new Personagem{
                        Nome = "Heiter",
                        TipoPersonagem = TiposPersonagem.Secundario,
                        Sinopse = "A priest who is a part of the Hero's party. He later took care of Fern until Frieren arrived.",
                        Foto = "3f39accf-c86f-456f-b4d9-7f7cf921bc9b.png",
                        DataNasc = null,
                        Idade = null,
                        PersonagemSexualidade = Sexualidade.Masculino
                    },
                    new Personagem{
                        Nome = "Himmel",
                        TipoPersonagem = TiposPersonagem.Secundario,
                        Sinopse = "Hero and Leader of the party that defeated the Demon King, and a self-proclaimed handsome narcissist. He cares about his friends and can't help but help those in need. He had a great influence on Frieren, with whom he had adventures for 10 years.",
                        Foto = "e285e0fa-b3a8-4bf4-a011-8da74dc16e02.png",
                        DataNasc = null,
                        Idade = null,
                        PersonagemSexualidade = Sexualidade.Masculino
                    },
                    new Personagem{
                        Nome = "Eisen",
                        TipoPersonagem = TiposPersonagem.Secundario,
                        Sinopse = "Dwarf warrior who was a part of the Hero's party, later became a mentor to Stark.",
                        Foto = "86bdbe7e-24e9-463d-b808-9ef2def8bb8f.png",
                        DataNasc = null,
                        Idade = null,
                        PersonagemSexualidade = Sexualidade.Masculino
                    },
                    new Personagem{
                        Nome = "Sakura Kinomoto",
                        TipoPersonagem = TiposPersonagem.Protagonista,
                        Sinopse = "Sakura is an extremely energetic and cheerful character. She is very athletic, being a member of her school's cheerleading squad and an excellent runner. She loves Physical Education and Music, but she really dislikes Math. Her \"invincible spell,\" \"Everything will surely be all right\" (「絶対大丈夫だよ」, \"Zettai daijoubu da yo\"), has carried her through innumerable trials and obstacles as she masters her magical skills. She often comes across as naive, clumsy, and clueless, but she is perceptive, sweet and loyal. She has a best friend named Tomoyo and is often recorded by her in action. She is cute and caring and has a big brother that is a chick magnet. She has a pretty normal life, except that she is the Cardcaptor, responsible for collecting all the Clow Cards, and also knows a magical \"Beast of the Seal\" named Kero who loves sweets.\n\nHer favorite colors are pink and white, and her favorite flower the cherry blossom (sakura). She's really good at cooking pancakes, but her favorite dishes are ebi fry, fried rice with eggs, and noodles. Sakura can't stand konnyaku.",
                        Foto = "92c25f28-c65f-4339-9aab-5d92b934f659.png",
                        DataNasc = null,
                        Idade = 12,
                        PersonagemSexualidade = Sexualidade.Feminino
                    },
                    new Personagem{
                        Nome = "Cerberus",
                        TipoPersonagem = TiposPersonagem.Protagonista,
                        Sinopse = "Cerberus, nicknamed \"Kero\", is the appointed guardian of the book which holds the Clow Cards. He is one of two magical creatures created by Clow Reed along with the Clow Cards. Before his death, Clow appointed Kero as the one to select the potential candidate to be the next master of the cards, Cerberus himself, and his \"brother\" and fellow guardian, Yue. After Sakura accidentally releases the cards, Cerberus chooses her to be the candidate and teaches her the basics of capturing the cards. Throughout the series, he displays an extensive knowledge of mysticism.\n\nHaving spent a lengthy amount of time in the book while it was in Osaka, Cerberus speaks with a pointed Osakan accent. He tends to be bossy, demanding, egotistical, and glutton, but clearly displays his affection for Sakura, especially if she is hurt or in danger. He becomes very fond of video games and is addicted to sweets. Cerberus spends most of his time in a \"false form\", a small figure resembling an orange-stuffed animal with wings.\n\nUnlike Yue, Cerberus draws his life energy from the sun and is not dependent on the magical power of his master. However, his master must have control of the Firey and Light cards in order to power his true form, a large-winged cat. In the anime adaptation, the Firey card is changed to the Earthy card to avoid delaying his obtaining his true form. Cerberus' name is taken from the Greek mythological figure, Cerberus, a large three-headed beast who was assigned to guard the gates of Hades.",
                        Foto = "e214ee0d-69ea-4ce6-8e97-54d0e2980ba4.png",
                        DataNasc = null,
                        Idade = null,
                        PersonagemSexualidade = Sexualidade.Masculino
                    },
                    new Personagem{
                        Nome = "Xiaolang Li",
                        TipoPersonagem = TiposPersonagem.Protagonista,
                        Sinopse = "Syaoran is a distant relative of Clow Reed, the creator of the Clow Cards. As a member of the Li clan of sorcerers from Hong Kong, of which Clow's mother was a member, Syaoran believes that he should be the one to inherit the Clow Cards rather than Sakura Kinomoto. He first appears as an antagonist in the story, capturing a few cards for himself in the anime (though none in the manga).\n\nAs the series progresses, and especially after Sakura is officially deemed the new master of the Cards, Syaoran drops his rivalistic attitude towards her and becomes her ally and friend, eventually falling in love with her and blushing at practically every sight of her.",
                        Foto = "84feec63-131f-4c82-9476-aa66e0d0db0f.png",
                        DataNasc = null,
                        Idade = 12,
                        PersonagemSexualidade = Sexualidade.Masculino
                    },
                    new Personagem{
                        Nome = "Tomoyo Daidouji",
                        TipoPersonagem = TiposPersonagem.Protagonista,
                        Sinopse = "Tomoyo (Madison Taylor in the English dub) is the best friend of the series heroine, Sakura Kinomoto. When she discovers that Sakura has become the Cardcaptor, she becomes Sakura's primary assistant by designing \"battle costumes\" and filming all of her magical (and non-magical) endeavours. (The videotaping, however, seems to be more to Tomoyo's personal benefit and enjoyment than it is to Sakura's.) Tomoyo faithfully keeps Sakura's new identity secret and often covers for her in times of need. Tomoyo is very mature for her age. One of her major character traits is her selflessness towards Sakura. Tomoyo is kind, caring, highly intelligent, beautiful, meticulous, and very melodic (she has a superb singing voice and often has solos in choir recitals). She always speaks using formal verb conjugations and expressions, giving her a unique air of refinement amongst the cast. When needed, Tomoyo can exhibit considerable cunning and resourcefulness, which was most prominently displayed in the final movie.",
                        Foto = "eb07db53-f509-4e9e-a513-0b18c631da6e.png",
                        DataNasc = null,
                        Idade = 12,
                        PersonagemSexualidade = Sexualidade.Feminino
                    },
                    new Personagem{
                        Nome = "Touya Kinomoto",
                        TipoPersonagem = TiposPersonagem.Secundario,
                        Sinopse = "In Cardcaptor Sakura Touya is the brother of Sakura Kinomoto. His birthday is on February 29th, which makes him a leap year baby, a person that can only \"properly\" celebrate their birthday once every four years. He holds the belief that as Sakura's older brother, he is the only person entitled to make fun of her, a belief that his best friend Yukito Tsukishiro refers to as a \"sister complex.\" However, he is also protective and caring of her, which leads him to dislike Syaoran Li when he finds Syaoran cornering Sakura in the schoolyard. Though Syaoran and Sakura eventually stop fighting, Toya continues to dislike Syaoran when he becomes friends with Sakura, knowing that Syaoran will eventually take Sakura away from him. He is a perceptive young man and has had a vague idea as to what Sakura has been up to as a Cardcaptor, despite Sakura's attempts to keep it a secret. Toya's main relationship is with his best friend Yukito. Throughout the series Toya and Yukito are extremely close and eventually Yukito openly admits his love for Toya to Sakura after Toya saves his life.Toya was born with some magical ability, allowing him to see ghosts (including the one of his late mother). His magical ability is also what helps him sense when Sakura is in danger or when she is doing other \"magical\" things (which enables him to follow her). - [ Taken from Wikipedia ] In Tsubasa Chronicles: Touya is the current king of the Clow Country, but he acts pretty much the same as he did in Cardcaptor Sakura",
                        Foto = "2b3bee65-ad30-409a-aba3-6243ca578ef6.png",
                        DataNasc = null,
                        Idade = 19,
                        PersonagemSexualidade = Sexualidade.Masculino
                    },
                    new Personagem{
                        Nome = "Yukito Tsukishiro",
                        TipoPersonagem = TiposPersonagem.Secundario,
                        Sinopse = "From the moment Yukito is introduced, it is clear that Sakura has an enormous crush on him. She continually marvels that he can be friends with her \"barbaric brother.\"\n\nEveryone involved, including Yukito himself, believes that his meeting Toya in junior high school three years ago had been coincidence. However, Yukito's nature as the false form of Yue also means that many of the things he thought he knew about himself are false. He lives in an elaborate house with his grandparents (the house is real, but the grandparents do not exist). In fact, it is unclear whether or not Yukito himself existed at all before his meeting with Toya and introduction to Sakura.\n\nYukito is peripherally involved in many of Sakura's adventures as the Cardcaptor, mainly because of Sakura's crush on him and his close proximity to the Kinomoto family. In the process, Yukito becomes the object of another juvenile crush; this one held by Syaoran Li. However, it is discovered later that Syaoran was just attracted to the magical energies of the moon residing within Yukito, as Syaoran draws his magical power from the moon.\n\nNot until the last Clow Card is collected does Yukito's true form, Yue, emerge and challenge Sakura's right to the cards. Afterwards, the Yukito and Yue personalities coexist, with Yukito unaware of Yue's presence.\n\nUnfortunately, the strain on Yue's energies becomes so great that even Yukito's massive consumption of food can no longer support it. Yukito begins to fall ill, becoming drowsy, falling asleep at the slightest opportunity, and eventually beginning to fade away altogether. The only thing that can save him is a massive infusion of magical energy, which Yue finally receives from Toya (whose earlier efforts to warn Yukito of his plight were continually interrupted by the aggressive Nakuru Akizuki).\n\nSoon afterwards, Yukito listens to and very gently declines Sakura's confession. He has always been aware of Sakura's feelings but tells her that he believes they are more platonic (as Yukito resembles Sakura's father) than romantic. Sakura immediately realizes that \"the one (Yukito) likes best\" is in fact Toya. Earlier that day, Yukito had told Toya how alarmed he was that he had suddenly learned about the existence of the Yue identity and the falsehood of most of his memories, but Toya replies that it doesn't matter to him since the moments they have shared together are real. After Yukito rejects Sakura the two of them seem to adopt a brother-sister type of relationship.\n\nIn the manga, Yukito is last seen in the Kinomoto kitchen with Toya, helping to prepare breakfast. There's a similar scene in the anime series, where they're preparing lunch. This somewhat implies that Toya has come to fully understand Yukito's feelings for him and reciprocates them.",
                        Foto = "96f8c473-bb0a-4ec3-9c58-448f4137c27d.png",
                        DataNasc = null,
                        Idade = 19,
                        PersonagemSexualidade = Sexualidade.Masculino
                    },
                    new Personagem{
                        Nome = "Kazuto Kirigaya",
                        TipoPersonagem = TiposPersonagem.Protagonista,
                        Sinopse = "Kirito is a \"solo\" player, a player who hasn't joined a guild and usually works alone. He is also one of the very few people to have had the privilege to play in the beta testing period of Sword Art Online. His game alias, Kirito, is created by taking the syllables of the first and last Kanji of his real last and first names respectively: (Kirigaya Kazuto).\n\nIn the real world, he lives with his mother and younger sister in a family of 3. When Kayaba announced the start of the death game, he is surprised, but unlike everyone else he quickly gets over it and accepts it to an extent. He invited his friend Klein to go with him but Klein had to find his friends in the game. He was invited to join them but he declined as he couldn't take the pressure of protecting them even with his beta testing knowledge of the game.",
                        Foto = "d11520dc-62ba-48bb-a78b-fc15d9171dba.png",
                        DataNasc = DateTime.Parse("2008-10-07 00:00:00"),
                        Idade = 16,
                        PersonagemSexualidade = Sexualidade.Masculino
                    },
                    new Personagem{
                        Nome = "Asuna Yuuki",
                        TipoPersonagem = TiposPersonagem.Protagonista,
                        Sinopse = "Asuna is a friend of Kirito and is a sub-leader of the guild Knights of the Blood (KoB), a medium-sized guild of about thirty players, also called the strongest guild in Aincrad. Being one of the few girls that are in SAO, and even more so that she's extremely pretty, she receives many invitations and proposals. She is a skilled player earning the title \"Lightning Flash\" for her extraordinary sword skill that is lightning fast. Her game alias is the same as her real world name.",
                        Foto = "30e336e9-41e5-40f4-b2df-f15e7121be15.png",
                        DataNasc = DateTime.Parse("2007-09-30 00:00:00"),
                        Idade = 17,
                        PersonagemSexualidade = Sexualidade.Feminino
                    },
                    new Personagem{
                        Nome = "Suguha Kirigaya",
                        TipoPersonagem = TiposPersonagem.Protagonista,
                        Sinopse = "Kazuto's sister in the real world. She is 15 years old and in her third year of junior high, as well as a member of the Kendou club. She's a diligent worker and has been practicing Kendou for 8 years.\n\nIn the VRMMO game Alfheim Online, she is known as Leafa, a Sylph fairy. She stumbled onto Kazuto by accident one night and decided to travel with him to the World Tree. She eventually fell in love with him because he showed that he really loves playing VR games as if living in it and is very honest in everything. In the real world she slowly got closer to her brother (actually a cousin) but because they're family and because he already got Asuna as a girlfriend she decided to give up on him and fall in love with Kazuto instead. But soon afterwards she realized that they're the same person. After being convinced by Kazuto to have a fierce duel which ended up in them flying in each others' arms, she decided to help Kazuto in rescuing Asuna.",
                        Foto = "932886a4-6e95-4ad8-90f7-fd8ea717bdf9.png",
                        DataNasc = DateTime.Parse("2009-04-09 00:00:00"),
                        Idade = 15,
                        PersonagemSexualidade = Sexualidade.Feminino
                    },
                    new Personagem{
                        Nome = "Akihiko Kayaba",
                        TipoPersonagem = TiposPersonagem.Antagonista,
                        Sinopse = "Director and creator of the Nerve Gear and Sword Art Online. He traps all the players within Sword Art Online by using the Nerve Gear to cease sending signals to the users' body. Any attempt of removal, loss of power for a certain period, or if the user dies in the game will also kill the user. His motivation is to observe what happens after he traps them.\n\nHe is in fact Heathcliff in the game SAO, who is the leader of the guild Knights of Blood.",
                        Foto = "855c8d0c-28b5-42d8-ac65-e39d9264d278.png",
                        DataNasc = null,
                        Idade = 28,
                        PersonagemSexualidade = Sexualidade.Masculino
                    },
                    new Personagem{
                        Nome = "Ryoutarou Tsuboi",
                        TipoPersonagem = TiposPersonagem.Secundario,
                        Sinopse = "A friendly player that meets Kirito at the start of the game and quickly befriends and learns from him. He and Kirito later separate because he has friends who were also new to the game so he couldn't leave them. He survives for the past two years and managed to protect his friends the whole time, something Kirito regretted not doing. He is a katana user and the leader of the small guild Fuurinkazan who participates in the boss battles. As one of the few people close to Kirito, he always keeps him in check and wants Kirito to survive the game. He chose the Salamader race on ALO\n\nIn the side story Caliber, he fell in love at first sight with the girl on the cage named Freyja and agreed that he will help her find her tribe's treasure which is a hammer, though the decision still remains to be a team's decision. The team said yes on Caliber and it turned out that the girl they saved was Thor. It was revealed that Thor disguised as a beautiful girl to get close to Giant King Þrym and retrieve his hammer. Thor defeated Giant King Þrym and that stopped the medallion from being corrupted and ending the quest.",
                        Foto = "4d981ad8-9159-4634-86f1-49d4bc7a42d3.png",
                        DataNasc = null,
                        Idade = 22,
                        PersonagemSexualidade = Sexualidade.Masculino
                    },
                    new Personagem{
                        Nome = "Shouzou Yuuki",
                        TipoPersonagem = TiposPersonagem.Figurante,
                        Sinopse = "Asuna's father. He is a CEO of a major electronics manufacturing company.",
                        Foto = "e4e4654f-fba2-43d2-8c84-ae3b40074faa.png",
                        DataNasc = null,
                        Idade = null,
                        PersonagemSexualidade = Sexualidade.Masculino
                    },
                    new Personagem{
                        Nome = "Naruto Uzumaki",
                        TipoPersonagem = TiposPersonagem.Protagonista,
                        Sinopse = "Born in Konohagakure, a ninja village hidden in the leaves, Naruto Uzumaki was destined for greatness. When born, a powerful nine-tailed demon fox attacked his village. With a wave of its tail, the demon fox could raise tsunamis and shatter mountains. In a valiant attempt to save the village from destruction, the Fourth Hokage and leader of the Hidden Leaf Village sealed the demon fox within Naruto's newborn body. This was his final act, for the battle with the fox cost him his life. Despite the Fourth Hokage's dying wish that Naruto is viewed as a hero for serving as the container for the demon (a Jinchuuriki), the adult villagers of Konoha harbored a fierce hatred for him, with many believing that Naruto and the demons were one and the same. Cast aside as an inhuman monster, Naruto was outcast and ostracised by the villagers for reasons he could not understand. The children his age could only ever follow their parents' example; and they too came to harbor a fierce hatred for Naruto. Naruto eventually came to accept that he would live and die alone, and his external response was to perform harmless pranks on the village. Coy, raffish, and full of life, Naruto soon came to display a somewhat unexpected determination to succeed and be accepted by others. Upon being assigned to \"Team Seven\" as a Genin-ranked ninja, his true potential soon became outwardly apparent. Vowing to become Hokage one day and using his will to never give in, Naruto saves the village from invading forces and earns his acceptance. Eventually, Naruto learns to harness the power of the Demon Fox sealed inside him to perform acts of strength far beyond what any other human is capable of. In all, Naruto is an admirable character whose sheer determination to succeed despite the odds, earns him respect and devotion from his fellow villagers.",
                        Foto = "a8f5aa0c-fea3-4d97-a136-997d5fddfc78.png",
                        DataNasc = null,
                        Idade = 17,
                        PersonagemSexualidade = Sexualidade.Masculino
                    },
                    new Personagem{
                        Nome = "Sasuke Uchiha",
                        TipoPersonagem = TiposPersonagem.Protagonista,
                        Sinopse = "When Sasuke was young his clan was murdered by his older brother, Itachi. With his parents and family now gone his brother spared him and left him to survive by himself. He told Sasuke to hate him. Sasuke applied to the ninja academy when he was young in order to get strong and avenge his clan by killing his brother. As he made friends on the way he forgot that this was his initial purpose. After his fight with Naruto he believes he's been wasting his time 'playing ninja' with the people of the leaf village. His new goal is to get stronger so he can avenge his clan by killing Itachi. As a wielder of the Sharingan (the kekkei genkai of the Uchiha clan), he learns to use Chidori and has a vast knowledge of the various Fire Jutsus.",
                        Foto = "61003f88-450c-4102-a1a2-49c81bf7baf7.png",
                        DataNasc = null,
                        Idade = 17,
                        PersonagemSexualidade = Sexualidade.Masculino
                    },
                    new Personagem{
                        Nome = "Sakura Haruno",
                        TipoPersonagem = TiposPersonagem.Protagonista,
                        Sinopse = "Sakura is the only female ninja of Team 7. When she first meets Naruto she is physically weak and has an immense crush on Sasuke. She dislikes Naruto as well. The only thing she's good at is her brain as the smartest girl in ninja academy her excellent chakra control, out of that she could be categorized as a kunoichi with no exceptional talent. In the first part of the Chuunin exam, she is easily able to answer the questions, but in the second part, she is unable to defeat the Sound ninja attacking Sasuke and Naruto. She makes a pledge to become a better ninja and a better person. To prove her commitment she cuts her long hair with a kunai. In the third part of the Chuunin Exam, she goes up against Ino Yamanaka, her former friend but a current rival for Sasuke's affections. The two defeat each other simultaneously. This turns their relationship into a rivalry between friends. After the time skip, she is now a chuunin having been trained by Tsunade for the past two years.\n\nShe is now a medic Ninja of considerable skill, enough skill to possibly defeat Tsunade one day. It is shown when she is able to do a complicated surgery to extract poisons out of Kankuro body and make an antidote of the new type of poison that Sasori used, which the Suna gakure medical-nin unable to do. During the time skip she's also matured in personality, she becomes calmer compare to when she was Genin, but she still has her temper. Sakura has grown not only as a medical-nin, but now she's also a capable fighter. She can manage to destroy the ground with her fist using her inhuman strength, a chakra control technique she learned from Tsunade, and she is also able to beat Sasori from Akatsuki together with elder Chiyo from Sunagakure. Additionally, she is a genjutsu type, and though she has yet to use that potential she does show an almost complete immunity to being trapped in an illusion.",
                        Foto = "77f2162e-2efd-4a13-84b1-724e13e7a418.png",
                        DataNasc = null,
                        Idade = 16,
                        PersonagemSexualidade = Sexualidade.Feminino
                    },
                    new Personagem{
                        Nome = "Kakashi Hatake",
                        TipoPersonagem = TiposPersonagem.Protagonista,
                        Sinopse = "Kakashi Hatake is a shinobi of Konohagakure's Hatake clan. Famed as Kakashi of the Sharingan and the Copy Ninja, he is one of Konoha's most talented ninjas, regularly looked to for advice and leadership despite his personal dislike of responsibility.\n\nKakashi has an ongoing, albeit slightly one-sided, rivalry with Might Guy, with Guy constantly proclaiming that Kakashi is his rival, and considering his and Kakashi's subordinates rivals. Kakashi, however, seems indifferent to their rivalry, which annoys Guy to no end.\n\nThough he evasively says that he has \"many hobbies\" when asked about himself, he is commonly seen reading of \"Icha Icha Paradise\" (literally \"Make Out Paradise\"), an adult and probably pornographic novel that is a runaway bestseller in the Naruto world. Kakashi most prominently reads it while training and speaking with his team, and is later seen reading the second known volume in the series, \"Icha Icha Violence\" (literally \"Make Out Violence\").",
                        Foto = "4e21ed15-82dd-426e-930a-b01f31a618df.png",
                        DataNasc = null,
                        Idade = 25,
                        PersonagemSexualidade = Sexualidade.Masculino
                    },
                    new Personagem{
                        Nome = "Itachi Uchiha",
                        TipoPersonagem = TiposPersonagem.Antagonista,
                        Sinopse = "Itachi Uchiha is a missing-nin from Konohagakure, and a prominent member of Akatsuki, partnered with Kisame Hoshigaki. He is Sasuke Uchiha's older brother. Ever since his first appearance, Itachi had always been mysterious, and had acted as though he was hiding his true self. While flashbacks to his past showed that he was a compassionate brother and person, his later acts and claims made it seem that this was mostly just an act.",
                        Foto = "462a418a-b217-4eb2-9172-c59e18496004.png",
                        DataNasc = null,
                        Idade = 21,
                        PersonagemSexualidade = Sexualidade.Masculino
                    },
                    new Personagem{
                        Nome = "Rock Lee",
                        TipoPersonagem = TiposPersonagem.Secundario,
                        Sinopse = "Lee is a ninja affiliated with the village of Konohagakure, and is a member of Team Guy, which consists of himself, Neji Hyuga, Tenten, and Might Guy—the team''s leader. Unable to use most ninja techniques, Lee dedicates himself to using solely taijutsu, ninja techniques similar to martial arts. Lee dreams of becoming a \"splendid ninja\" despite his inabilities",
                        Foto = "87c2012e-5e5f-40c8-aaf7-ed4df76f2e7f.png",
                        DataNasc = null,
                        Idade = null,
                        PersonagemSexualidade = Sexualidade.Masculino
                    }
                ];

                await dbContext.Personagens.AddRangeAsync(personagens);
                await dbContext.SaveChangesAsync();
                haAdicao = true;
            }
            else
            {
                personagens = await dbContext.Personagens.ToArrayAsync();
            }



            // Código para inserir os dados dos estúdios no banco de dados
            var studios = Array.Empty<Studio>();
            if (!dbContext.Studios.Any())
            {
                studios = [
                    new Studio
                    {
                        Nome = "MADHOUSE",
                        Sobre = "Madhouse (MADHOUSE Inc.) is a Japanese animation studio based in Nakano City, Tokyo. Ex-Mushi Production animators—including Masao Maruyama, Osamu Dezaki, Rintarou, and Yoshiaki Kawajiri—are often credited with founding the company in 1972, though Rintarou would not join the studio until 1982. Madhouse primarily did contract work for other studios until the 1990s, when it achieved success independently. Television adaptations of Trigun and Clamp's Cardcaptor Sakura in 1998 were hits domestically and later aired overseas, and the same year director Satoshi Kon's Perfect Blue was released as his first of four critically-acclaimed films with the company.\n\nMadhouse remained successful throughout the 2000s and 2010s, adding director Mamoru Hosoda to its roster and releasing numerous popular television anime, including adaptations of Hajime no Ippo, Death Note, and One Punch Man. Maruyama stepped down from his role as president and left the company in June 2011 to start studio MAPPA.",
                        Status = Estado.Ativo,
                        Fechado = null,
                        Fundado = DateTime.Parse("1972-10-17 00:00:00"),
                        Foto = "c329ee87-1700-4453-9631-c5140bcac536.png"
                    },
                    new Studio
                    {
                        Nome = "Wit Studio",
                        Sobre = "Wit Studio, Inc., stylized as WIT Studio, is a Japanese animation studio founded on June 1, 2012, by producers at Production I.G as a subsidiary of IG Port. It is headquartered in Musashino, Tokyo, with Production I.G producer George Wada as president and Tetsuya Nakatake, also a producer at Production I.G., as a director of the studio.\n\nThe studio was founded by George Wada, a former employee of Production I.G, in 2012. After its founding, Tetsuya Nakatake was placed as the representative director of the studio. Several other former Production I.G staff members joined Wit after its founding, including animation directors Kyōji Asano and Satoshi Kadowaki, and director Tetsurō Araki, all of whom worked together on Attack on Titan.\n\nWit Studio was funded with an initial investment of ¥30,000,000 in capital from IG Port, Wada and Nakatake, who are reported to own 66.6%, 21.6% and 10.0% equity in the studio respectively.\n\nIn December 2020, Wit Studio established a stop motion studio in partnership with Pui Pui Molcar director Tomoki Misato.\n\nIn May 2022, Wit Studio in partnership with Aniplex, CloverWorks and Shueisha formed a new company called JOEN. The company's objective is to facilitate the planning and production of television anime series, anime films, and short clips.\n\n(Source: Wikipedia)",
                        Status = Estado.Ativo,
                        Fechado = null,
                        Fundado = DateTime.Parse("2012-06-01 00:00:00"),
                        Foto = "60e6cca7-e80b-4145-b958-a9c4005b4acc.png"
                    },
                    new Studio
                    {
                        Nome = "A-1 Pictures",
                        Sobre = "A-1 Pictures (A-1 Pictures Inc.) is a Japanese animation studio in Suginami, Tokyo. Founded by former Sunrise producer Mikihiro Iwata in 2005, it was established as a subsidary of Aniplex. The studio was meant to oversee Aniplex's family-oriented series before it evolved into producing various independent anime projects.\n\nA-1 Pictures has since released popular television anime such as Fairy Tail, Sword Art Online, Nanatsu no Taizai (The Seven Deadly Sins), and Kaguya-sama wa Kokurasetai: Tensai-tachi no Renai Zunousen (Kaguya-Sama: Love is War).",
                        Status = Estado.Ativo,
                        Fechado = null,
                        Fundado = DateTime.Parse("2005-03-09 00:00:00"),
                        Foto = "087ccf29-eb6f-4cbb-95fa-6f27ccf59158.png"
                    },
                    new Studio
                    {
                        Nome = "Studio Pierrot",
                        Sobre = "Pierrot ぴえろ (Pierrot Co., Ltd.) is a Japanese animation studio established in May 1979 by former employees of both Tatsunoko Production and Mushi Production. Its headquarters are located in Mitaka, Tokyo. Pierrot is renowned for several worldwide popular anime series, such as Naruto, Bleach, Yu Yu Hakusho, Black Clover, Boruto: Naruto Next Generations, Tokyo Ghoul, and Great Teacher Onizuka.",
                        Status = Estado.Ativo,
                        Fechado = null,
                        Fundado = DateTime.Parse("1979-05-01 00:00:00"),
                        Foto = "0921c2f6-49cc-483c-a1d3-55d081b1080a.png"
                    },
                    new Studio
                    {
                        Nome = "Bones",
                        Sobre = "Bones (Bones Inc.) is a Japanese animation studio based in Suginami, Tokyo. The studio was founded by previous Sunrise producer Masahiko Minami and animators Hiroshi Ousaka and Toshihiro Kawamoto in 1998. Following Sunrise's production model, Bones' founders divided the company into five smaller studios, Studio A-E.\n\nStudio Bones has put out a variety of television and film since its debut project of Hiwou War Chronicles in 2000, including popular anime such as Ouran Koukou Host Club (Ouran High School Host Club), Hagane no Renkinjutsushi: Fullmetal Alchemist (Fullmetal Alchemist: Brotherhood), Boku no Hero Academia (My Hero Academia), and Bungou Stray Dogs",
                        Status = Estado.Ativo,
                        Fechado = null,
                        Fundado = DateTime.Parse("1998-10-01 00:00:00"),
                        Foto = "cf7410d3-cc69-4613-a7a9-9a47324f860e.png"
                    }
                ];

                await dbContext.Studios.AddRangeAsync(studios);
                await dbContext.SaveChangesAsync(); // Save studios first
                haAdicao = true;
            }
            else
            {
                // If studios already exist, load them from database
                studios = await dbContext.Studios.ToArrayAsync();
            }

            // Código para inserir os dados dos autores no banco de dados
            var autores = Array.Empty<Autor>();
            if (!dbContext.Autores.Any())
            {
                autores = [
                    new Autor
                    {
                        Nome = "CLAMP",
                        DateNasc = null,
                        Sobre = "CLAMP is an all-female Japanese manga artist group that formed in the mid-1980s. It consists of leader Nanase Ohkawa, and three artists who shift roles for each series: Mokona (もこな), Tsubaki Nekoi, and Satsuki Igarashi (いがらし寒月). Almost 100 million CLAMP tankōbon copies have been sold worldwide as of October 2007.\n\nIn general, Ohkawa gets her inspiration for the group from everyday events such as dreams or the news. Unlike most manga artists who specialize in a single genre, CLAMP has created a diverse body of work. CLAMP's genres vary widely, from childish and comedic (Cardcaptor Sakura, Clamp School Detectives) to more dramatic and teen-rated (xxxHolic, X) series. Furthermore, drawing from the idea of Osamu Tezuka's Star System as they did in Tsubasa: Reservoir Chronicle, CLAMP often crossover characters from their own series into their other works, which gives rise to a loosely defined \"Clamp Universe\".",
                        Foto = "21d9e142-6e5f-493f-bb56-e7957167d5d8.png",
                        Idade = null,
                        AutorSexualidade = null
                    },
                    new Autor
                    {
                        Nome = "Kanehito Yamada",
                        DateNasc = null,
                        Sobre = "Não possui biografia.",
                        Foto = "2c4a3224-495d-4be5-b900-4344d2493740.png",
                        Idade = null,
                        AutorSexualidade = null
                    },
                    new Autor
                    {
                        Nome = "Hajime Isayama",
                        DateNasc = DateTime.Parse("1986-08-29 00:00:00"),
                        Sobre = "After graduating from Oita Prefectural Hitarinkou High School, he matriculated in the cartoon design program of the cartoon arts department at Kyushu Designer Gakuin. After graduating, he moved to Tokyo and started drawing his manga works.\n\nIn 2006, he applied for the Magazine Grand Prix known as MGP promoted by Kodansha Ltd. and his work Attack on Titan was given the \"Fine Work\" award. Originally, he offered his work to the Weekly Shonen Jump department at Shueisha, where he was advised to modify his style and story to be more suitable for Weekly Shonen Jump. He declined and instead, decided to take it to the Weekly Shonen Magazine department at Kodansha Ltd.\n\nIn 2008, he applied for the 80th Weekly Shonen Magazine Freshman Manga Award, where his work [\"Heart Break One\"]( was given the Special Encouragement Award. His other work \"orz\" was chosen as a selected work in the 81st Weekly Shonen Magazine Freshman Manga Award.\n\nIn 2009, his first serialized work Attack on Titan appeared in Bessatsu Shonen Magazine, a monthly magazine. It later was awarded the Shonen category of the 35th Kodansha Manga Award in 2011.",
                        Foto = "4e6708ed-a532-481e-b190-efcc7961e5f4.png",
                        Idade = 38,
                        AutorSexualidade = Sexualidade.Masculino
                    },
                    new Autor
                    {
                        Nome = "Reki Kawahara",
                        DateNasc = DateTime.Parse("1974-08-17 00:00:00"),
                        Sobre = "Kawahara Reki graduated from the Aoyama Gakuin University.",
                        Foto = "ec4524c2-a3a2-4083-b768-b220d51a4c0f.png",
                        Idade = 50,
                        AutorSexualidade = Sexualidade.Masculino
                    },
                    new Autor
                    {
                        Nome = "Masashi Kishimoto",
                        DateNasc = DateTime.Parse("1974-11-08 00:00:00"),
                        Sobre = "Masashi Kishimoto is a Japanese manga artist, well known for creating the manga series Naruto. His younger twin brother, Seishi Kishimoto, is also a manga artist and creator of the manga series 666 Satan and Blazer Drive. He is good friends with Eiichirou Oda",
                        Foto = "34ae142d-83dc-467c-a6c8-d9fba6a4919a.png",
                        Idade = 50,
                        AutorSexualidade = Sexualidade.Masculino
                    },
                    new Autor
                    {
                        Nome = "Hiromu Arakawa",
                        DateNasc = DateTime.Parse("1973-05-08 00:00:00"),
                        Sobre = "Her self-portrait is usually that of a bespectacled cow, as she was born and raised in a dairy farm with three older sisters and a younger brother. Arakawa started out as Etou Hiroyuki's assistant writer for Mahoujin Guru Guru and a friend of Shakugan no Shana author Yashichiro Takahashi. Arakawa's career started with a work titled STRAY DOG, but she is best known for creating the Fullmetal Alchemist world and manga. The author is friends with Akira Segami.\n\nDue to a lack of real publicity photos of Arakawa, a great sum of her fanbase has no idea what she really looks like, even to this day. There are some scarce pictures of the real Arakawa in the web, but they are very low in quality and are rarely used as reference. Many fans have mistaken photos of Romi Park, Edward Elric's seiyuu, as photos of Arakawa, because of the set-up in one particular image in which Romi Park poses as a mangaka. Romi Park is known for representing Arakawa in public appearances, in which Arakawa is not able to attend.\n\nAwards:\n• 1999: 9th 21st Century Enix Award for \"Stray Dog\"\n• 2003: 49th Shogakukan Manga Award, Shōnen category for \"Fullmetal Alchemist\"\n• 2011: 15th Tezuka Osamu Cultural Prize, \"New Artist Prize\" category.\n• 2011: 42nd Seiun Award, \"Best Science Fiction Comic\" category for \"Fullmetal Alchemist\"\n• 2012: 5th Manga Taishō Award for \"Silver Spoon\"\n• 2012: 58th Shogakukan Manga Award, Shōnen category for \"Silver Spoon\"\n• 2013: 1st Contents Award of Japan Food Culture Grand Prize for \"Silver Spoon\"\n• 2023: 1st Rakuten Kobo's E-Book Manga Award, \"I Want to Read it Now! Featured Manga\" category for \"Yomi no Tsugai\"\n• 2023: 7th Tsutaya Comic Award for \"Yomi no Tsugai\"",
                        Foto = "92e54d43-8fc0-4bd6-8a29-cf0526686c70.png",
                        Idade = 52,
                        AutorSexualidade = Sexualidade.Feminino
                    }
                ];

                await dbContext.Autores.AddRangeAsync(autores);
                await dbContext.SaveChangesAsync(); // Save authors first
                haAdicao = true;
            }
            else
            {
                // If authors already exist, load them from database
                autores = await dbContext.Autores.ToArrayAsync();
            }

            // Se não houver Shows, cria-os
            var shows = Array.Empty<Show>();
            if (!dbContext.Shows.Any())
            {
                shows = [
                    new Show{
                    Nome = "Cardcaptor Sakura",
                    Sinopse = "One day, Kinomoto Sakura, a 4th grader stumbles upon the mysterious book of Clow. Upon opening it and reading the name of The Windy aloud, Sakura scatters the cards to the winds. Sakura is elected and appointed by Keroberos, Guardian of the Cards to capture the remaining cards. With her friend Tomoyo and rival Syaoran, Sakura begins an adventure that will forever change her.",
                    Status = Status.Finalizado,
                    Nota = 2.00m,
                    Ano = 1998,
                    Imagem = "cfe07492-1be8-47da-a564-c5768249394e.png",
                    Banner = "cfe07492-1be8-47da-a564-c5768249394e.png",
                    Trailer = "https://v.animethemes.moe/CardcaptorSakura-OP1.webm",
                    Views = 89341,
                    Fonte = Fonte.Manga,
                    StudioFK = studios[0].Id,
                    AutorFK = autores[0].Id,
                    DataCriacao = DateTime.Parse("2001-01-01 00:00:00"),
                    DataAtualizacao = null
                },
                new Show{
                    Nome = "Frieren: Beyond Journey's End",
                    Sinopse = "The adventure is over but life goes on for an elf mage just beginning to learn what living is all about. Elf mage Frieren and her courageous fellow adventurers have defeated the Demon King and brought peace to the land. But Frieren will long outlive the rest of her former party. How will she come to understand what life means to the people around her? Decades after their victory, the funeral of one her friends confronts Frieren with her own near immortality. Frieren sets out to fulfill the last wishes of her comrades and finds herself beginning a new adventure…\n\n(Source: Crunchyroll)",
                    Status = Status.Finalizado,
                    Nota = 9.10m,
                    Ano = 2023,
                    Imagem = "fde46464-45df-44bc-bcb7-3d33341a2b69.png",
                    Banner = "fde46464-45df-44bc-bcb7-3d33341a2b69.png",
                    Trailer = "https://v.animethemes.moe/SousouNoFrieren-OP1.webm",
                    Views = 314017,
                    Fonte = Fonte.Manga,
                    StudioFK = studios[0].Id,
                    AutorFK = autores[1].Id,
                    DataCriacao = DateTime.Parse("2001-01-01 00:00:00"),
                    DataAtualizacao = null
                },
                new Show{
                    Nome = "Attack on Titan",
                    Sinopse = "Several hundred years ago, humans were nearly exterminated by titans. Titans are typically several stories tall, seem to have no intelligence, devour human beings and, worst of all, seem to do it for the pleasure rather than as a food source. A small percentage of humanity survived by walling themselves in a city protected by extremely high walls, even taller than the biggest of titans.\n\nFlash forward to the present and the city has not seen a titan in over 100 years. Teenage boy Eren and his foster sister Mikasa witness something horrific as the city walls are destroyed by a colossal titan that appears out of thin air. As the smaller titans flood the city, the two kids watch in horror as their mother is eaten alive. Eren vows that he will murder every single titan and take revenge for all of mankind.\n\n(Source: MangaHelpers)",
                    Status = Status.Finalizado,
                    Nota = 4.00m,
                    Ano = 2013,
                    Imagem = "744b06c1-97f0-4e26-88e6-3ea75239bc2d.png",
                    Banner = "ed8dc479-8aa6-4075-ba0d-854a2bb6640c.png",
                    Trailer = "https://v.animethemes.moe/ShingekiNoKyojin-OP1.webm",
                    Views = 880013,
                    Fonte = Fonte.Manga,
                    StudioFK = studios[1].Id,
                    AutorFK = autores[2].Id,
                    DataCriacao = DateTime.Parse("2001-01-01 00:00:00"),
                    DataAtualizacao = null
                },
                new Show{
                    Nome = "Sword Art Online",
                    Sinopse = "In the near future, a Virtual Reality Massive Multiplayer Online Role-Playing Game (VRMMORPG) called Sword Art Online has been released where players control their avatars with their bodies using a piece of technology called Nerve Gear. One day, players discover they cannot log out, as the game creator is holding them captive unless they reach the 100th floor of the game's tower and defeat the final boss. However, if they die in the game, they die in real life. Their struggle for survival starts now...\n\n(Source: Crunchyroll)",
                    Status = Status.Finalizado,
                    Nota = 7.50m,
                    Ano = 2012,
                    Imagem = "51d02292-c4a4-4c79-8a40-ebb59a36a84a.png",
                    Banner = "51d02292-c4a4-4c79-8a40-ebb59a36a84a.png",
                    Trailer = "https://v.animethemes.moe/SwordArtOnline-OP1.webm",
                    Views = 615008,
                    Fonte = Fonte.LightNovel,
                    StudioFK = studios[2].Id,
                    AutorFK = autores[3].Id,
                    DataCriacao = DateTime.Parse("2001-01-01 00:00:00"),
                    DataAtualizacao = DateTime.Parse("2025-06-28 17:01:13")
                },
                new Show{
                    Nome = "Naruto",
                    Sinopse = "Naruto Uzumaki, a hyperactive and knuckle-headed ninja, lives in Konohagakure, the Hidden Leaf village. Moments prior to his birth, a huge demon known as the Kyuubi, the Nine-tailed Fox, attacked Konohagakure and wreaked havoc. In order to put an end to the Kyuubi's rampage, the leader of the village, the 4th Hokage, sacrificed his life and sealed the monstrous beast inside the newborn Naruto.\n\nShunned because of the presence of the Kyuubi inside him, Naruto struggles to find his place in the village. He strives to become the Hokage of Konohagakure, and he meets many friends and foes along the way.\n\n(Source: MAL Rewrite)",
                    Status = Status.Finalizado,
                    Nota = 7.90m,
                    Ano = 2002,
                    Imagem = "26f2aa0b-3ad2-47c4-a8ba-1a8af77f1cef.png",
                    Banner = "26f2aa0b-3ad2-47c4-a8ba-1a8af77f1cef.png",
                    Trailer = "https://v.animethemes.moe/Naruto-OP2.webm",
                    Views = 610318,
                    Fonte = Fonte.Manga,
                    StudioFK = studios[3].Id,
                    AutorFK = autores[4].Id,
                    DataCriacao = DateTime.Parse("2001-01-01 00:00:00"),
                    DataAtualizacao = DateTime.Parse("2025-06-28 21:55:59")
                },
                new Show{
                    Nome = "Naruto: Shippuden",
                    Sinopse = "Naruto: Shippuuden is the continuation of the original animated TV series Naruto. The story revolves around an older and slightly more matured Uzumaki Naruto and his quest to save his friend Uchiha Sasuke from the grips of the snake-like Shinobi, Orochimaru. After 2 and a half years Naruto finally returns to his village of Konoha, and sets about putting his ambitions to work, though it will not be easy, as he has amassed a few (more dangerous) enemies, in the likes of the shinobi organization; Akatsuki.\n\n(Source: Anime News Network)",
                    Status = Status.Finalizado,
                    Nota = 8.10m,
                    Ano = 2007,
                    Imagem = "abf5c2ea-6f30-4caf-89bb-9e15afb735d0.png",
                    Banner = "abf5c2ea-6f30-4caf-89bb-9e15afb735d0.png",
                    Trailer = "https://v.animethemes.moe/NarutoShippuuden-OP1.webm",
                    Views = 517723,
                    Fonte = Fonte.Manga,
                    StudioFK = studios[3].Id,
                    AutorFK = autores[4].Id,
                    DataCriacao = DateTime.Parse("2001-01-01 00:00:00"),
                    DataAtualizacao = DateTime.Parse("2025-06-28 22:03:47")
                },
                new Show{
                    Nome = "Fullmetal Alchemist",
                    Sinopse = "The rules of alchemy state that to gain something, one must lose something of equal value. Alchemy is the process of taking apart and reconstructing an object into a different entity, with the rules of alchemy to govern this procedure. However, there exists an object that can bring any alchemist above these rules, the object known as the Philosopher's Stone. The young Edward Elric is a particularly talented alchemist who through an accident years back lost his younger brother Alphonse and one of his legs. Sacrificing one of his arms as well, he used alchemy to bind his brother's soul to a suit of armor. This lead to the beginning of their journey to restore their bodies, in search for the legendary Philosopher's Stone.\n\nNote: Episodes 11, 12 and 37 were adapted from the light novel \"Fullmetal Alchemist: The Land of Sand.\"",
                    Status = Status.Finalizado,
                    Nota = 7.90m,
                    Ano = 2003,
                    Imagem = "6c446e19-0f3d-4b6a-bfaa-61018069c36c.png",
                    Banner = "6c446e19-0f3d-4b6a-bfaa-61018069c36c.png",
                    Trailer = "https://v.animethemes.moe/FullmetalAlchemist-OP1.webm",
                    Views = 227864,
                    Fonte = Fonte.Manga,
                    StudioFK = studios[4].Id,
                    AutorFK = autores[5].Id,
                    DataCriacao = DateTime.Parse("2001-01-01 00:00:00"),
                    DataAtualizacao = DateTime.Parse("2025-06-28 22:18:58")
                }
                ];

                await dbContext.Shows.AddRangeAsync(shows);
                await dbContext.SaveChangesAsync();
                haAdicao = true;
            }
            else
            {
                shows = await dbContext.Shows.ToArrayAsync();
            }

            if (haAdicao)
            {
                Console.WriteLine("Database seeded successfully.");
            }
        }
    }
}