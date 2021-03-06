### Rapport

####Webbteknik 2

###Inledning
Jag har skapat en mashup applikation vilken använder tre olika Apier. Två av apier används för inläsning av data, och en av Apierna (Google Maps) visar data på en karta. Applikation använder Instagram api för att visa Instagram media (bilderna och video) filtrerade efter taggs, eller efter geografisk läge.
###Schematisk bild över applikationens beståndsdelar

![schema](https://cloud.githubusercontent.com/assets/8629282/12969407/5c5a96d4-d080-11e5-825d-0fddc298199a.png)

###Tekniker
Jag använder Instagram API, Google maps API och Google places API. Jag har inte haft större problem med google Apier, delvist för att jag har redan använt denna i laboration 3, men också för att jag har hittat ganska bra tutorials.
Instagram Api var riktigt svårt att implementera. Instagram tillåter inte sökning av media på deras servis för användare vilka är inte inloggade. Inloggnings process är sådan (OAuth2), att man får en _code_ av Instagram (då när man skickar request och får response på definierad redirect.uri) och med koden man gör en request vilket resulterar att man får _access token_ vilken man kan använda i sökningarna. Dokumentation kring detta var inte av större hjälp, och jag hittade inga bra tutorials. Jag hittade dock, en wrappen gjort för Instagram, så att man kunde inkludera biblioteket i sitt projekt och utveckla vidare på smydigt sätt, men jag valde att gå min egen väg.
En till begränsning är sandbox mode, vilket gjorde att testning var riktigt svårt. Sandbox-mode begränsar sökresultat på 10 användare och 20 sista bilder av varje användare. Funktionalitet är samma som om man söker mot allt media, men man var tvungen att ha några användare i sin sandbox. Jag var tvungen skapa en test användare och ladda upp lite bilder men min demovideo visar inte applikation i bästa ljuset, då när man skriver adress på favoritställe och får en massa bilder laddade upp på samma stället.
Server sida är utvecklad i ASP.NET MVC.
En del funktionalitet på klient sida är utvecklad i JavaScript.
###Säkerhet och prestandaoptimering
Jag har publicerat applikation på Windows Azure, och när man publicerar applikation från Visual Studio på Azure körs minimizering automatiskt. Det resulterade att delar av mina javascripter slutade fungera. För att förbi detta problem, jag var tvungen att använda mig av:
```
 BundleTable.EnableOptimizations = false;
```
i min bundle fil, för att undvika automatiskt optimizering av Azure. Jag är säkert att detta påverkade prestandan negativt.
Jag försökte undvika onödiga requests mot Instagram API, då man söker media man kan få tillgång till ny access token varje gång. Det är bra att undvika så jag sparade alla access tokens i JSON format i en JSON fil. Jag funderade på om access tokens kan kompromitera användare på något sätt i fall att elaka användare får tillgång till min JSON fil, men Access token är del av Response vilken man får av Instagram och själva scopet för allt media är _public_ (access token är genererad bara för public scope).
När man registrerade sin klient på Instagram, man får några "nycklar". Den information sparar jag i webconfig fil (vilket är säkrast i ASP.NET applikationer) och laddar i koden bara då när jag behöver göra Request mot Instagram API.
###Offline-first:
Jag inkluderar en cache.manifest fil i HTML tagg. Filen innehåller instruktioner om vilka delar i applikation sparas lokalt (cache).
Jag har gjort en offline.html sida, vilken visas för användare i såna delar i applikationen som är inte cachad.
Själva applikation behöver internetuppkoppling för att hämta data från Instagram API, och det är huvudsyfte med applikation. Data från Google APIer laddas lokalt och är tillgängligt även om man tappar uppkoppling men kommer att försvinna när sidan laddas om.
###Risker med din applikation:
Apierna slutar fungera på något sätt eller blir korrupta. Själva inloggnigs process är gjort av Instagram, och jag använder "server flow" (så som Instagram döper detta) vilket är förklarad säkrare väg i Instagram dokumentation.

###Egen reflektion kring projektet:
Att utveckla den applikation tog väldigt mycket tid. Jag är ganska nöjd på hur organiserade jag mina model klasser. Jag har inte hunnit implementera alla funktioner som jag planerade men jag har väldigt bra grund för detta, eftersom jag har implementerat många GET funktioner som är används inte på slutet. Jag kommer utveckla vidare så att användare kan ställa flera andra frågor mot API. Jag förväntade att jag ska klara utveckla lite mer avancerad gränssnitt, men på slutet jag kan bli nöjd med det som finns just nu.
Modellen är starkast sida i mitt projekt som sagt. Det hade varit roligare att utveckla  om applikation inte jobbar i Sandbox-mode, vilket tar den glädje av utvecklaren när något fungerar och man får tillgång till allt media som finns. Jag hoppas att Instagram ska aktivera min applikation till hela Instagram media, och då blir det roligare att utveckla vidare.
Några funktioner som jag har inte hunnit implementera:
* Skriva en tagg, få tagg lista med liknande taggar. Man väljer en av lista och får resultat ( inte riktig Mashup funktion)
* Man väljer ett geografiskt läget på kartan genom att sticka en marker, och man får latitude och longitude och söker efter detta
* Man sticker marker på kartan, får en lista med intressanta lägge omkring (default 1 kilometer). Man trycker på ett läge i listan och får nya resultat
