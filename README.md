# Presentation Feedback Application
An application kata to apply Flow Design principles to a more elaborate scenario. Implementation in C# using ASP.NET MVC.

## Scenario
A web portal to give feedback to speakers at a conference.

A conference consists of a number of sessions. Attendees can give feedback to each session in a simple way: by selecting a color from a traffic light (red, yellow, green) and optionally adding a comment.

Sessions can be scored from 15min after they started until maybe 1h after they finished.

Attendees have to identify themselves with their email address upon feedback. (No further user management needed for a start.) Identification makes it easy to avoid multiple scoring for sessions by the same attendee.
When scoring for the first time, a confirmation email is sent to the attendee.

Sessions are registered with title, name and picture of speaker and email address. (Possibly the picture can be retrieved from [Gravatar](http://en.gravatar.com).) At the end of the feedback period, the speaker gets and email with the feedback.

Also the complete feedback for a conference can be downloaded at any time as a CSV file.

You can find a more elaborate description [here](https://app.box.com/s/43dhdqnn92xlezdxcx78) (German).

## Resources
Information on Flow Design can be found in Ralf Westphal's English blog ["The Architect's Napkin"](http://geekswithblogs.net/theArchitectsNapkin/default.aspx) or in his books at [Leanpub](https://leanpub.com/u/ralfw).
