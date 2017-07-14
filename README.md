# FSharp.ReaderM
An implementation of the Reader monad in F#

This project arose out of my desire to use this pattern in different projects, but not wanting to copy/paste it over
and over. Feel free to use it; PRs are welcome, as I am not a monad expert.

### Credits

The code here was created based on prior work from several individuals:
 - Carsten König and his "[Dependency Injection - A Functional Way][ck]" post
 - Matthew Podwysokci and his "[Much Ado about Monads - Reader Edition][mp]" post
 - Scott Wlaschin and his "[Reinventing the Reader Monad][sw]" post (and that entire series)
 - Chet Husk and his one-on-one feedback to [my first attempt to use this][ch]

_(Sorry, Scott - I'm still not ready to [write a monad tutorial][tut]...)_

[ck]:  http://gettingsharper.de/2015/03/10/dependency-injection-a-functional-way/
[mp]:  http://www.codebetter.com/matthewpodwysocki/2010/01/07/much-ado-about-monads-reader-edition/
[sw]:  http://fsharpforfunandprofit.com/posts/elevated-world-6/
[ch]:  https://gist.github.com/danieljsummers/eca04e64b903f08aecfb15e8f2536dd6
[tut]: http://fsharpforfunandprofit.com/posts/why-i-wont-be-writing-a-monad-tutorial/