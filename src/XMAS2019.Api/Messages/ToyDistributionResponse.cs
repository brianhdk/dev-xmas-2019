using System;
using XMAS2019.Domain;

namespace XMAS2019.Api.Messages
{
    public class ToyDistributionResponse
    {
        private static readonly Random Random = new Random(Guid.NewGuid().GetHashCode());

        private readonly Statistics _statistics;

        public ToyDistributionResponse(Statistics statistics)
        {
            _statistics = statistics ?? throw new ArgumentNullException(nameof(statistics));
        }

        public string Message => $@"{Greetings[Random.Next(0, Greetings.Length)]} 

Congratulations {_statistics.Participant.FullName}, you've completed Vertica Dev XMAS 2019 Christmas contest!. 

I hope you had fun playing around with RESTful APIs, Elasticsearch, Azure (Functions, Blobs and CosmosDB), all while saving Christmas!

I'll contact you by e-mail ('{_statistics.Participant.EmailAddress}'), if you win any prizes. 

Also please make sure to pay close attention to any updates on https://blog.vertica.dk
 - we'll reveal winners on Friday 20th December once the contest is done.

With best wishes for a Merry Christmas and a Happy New Year

 - Brian Holmgård Kristensen / bhk@vertica.dk";

        public long TotalNumberOfAttempts => _statistics.NumberOfAttempts;
        public TimeSpan ExecutionTime => _statistics.LastAttempt.CalculateExecutionTime();

        private static readonly string[] Greetings = 
        {
            "B-E-A-utiful!",
            "Groovy!",
            "Awesome!",
            "Super!",
            "That's right!",
            "Good work!",
            "Exactly right.",
            "You're doing a good job.",
            "That's it!",
            "Now you've figured it out.",
            "Great!",
            "I knew you could do it.",
            "Congratulations!",
            "Not bad.",
            "Now you have it!",
            "Good for you!",
            "Couldn't have done it better myself.",
            "Aren't you proud of yourself?",
            "You really make my job fun.",
            "That's the right way to do it.",
            "You did it that time!",
            "That's not half bad.",
            "Nice going.",
            "You haven't missed a thing!",
            "Wow!",
            "That's the way!",
            "Keep up the good work.",
            "Terrific!",
            "Nothing can stop you now.",
            "That's the way to do it.",
            "Sensational!",
            "You've got your brain in gear today.",
            "That's better.",
            "That was first class work.",
            "Excellent!",
            "That's the best ever.",
            "You've just about mastered it.",
            "Perfect!",
            "That's better than ever.",
            "Much better!",
            "Wonderful!",
            "You must have been practicing.",
            "You did that very well.",
            "Fine!",
            "Nice going.",
            "You're really going to town.",
            "Outstanding!",
            "Fantastic!",
            "Tremendous!",
            "That's how to handle that.",
            "Now that's what I call a fine job.",
            "That's great.",
            "Right on!",
            "You're really improving.",
            "You're doing beautifully!",
            "Superb!",
            "Congratulations. You got it right!",
            "Marvelous!",
            "Way to go!",
            "Good thinking.",
            "That's really nice."
        };
    }
}