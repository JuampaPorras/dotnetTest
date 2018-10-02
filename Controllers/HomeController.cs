using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using test.Models;

namespace test.Controllers
{
    public class HomeController : Controller
    {
        // private String FindTag(string text){
        // String leftoverText="";
        // int currentOpenTag=text.IndexOf('<');
        // if(currentOpenTag>-1){
        //     int currentCloseTag=text.IndexOf('>',currentOpenTag);
        //     if(currentCloseTag>-1){
        //         String currentTag="";
        //         int currentTagIndex=0;
        //         String mirrorEndTag="";
        //         int mirrorEndTagIndex=0;
        //         //take the initial text that is never going to appear as valid
        //         leftoverText=text.Substring(text.IndexOf('<'));
        //         currentOpenTag=leftoverText.IndexOf('<');
        //         currentCloseTag=leftoverText.IndexOf('>');
        //         //find the value of the first tag and find if it has any matching endTag
        //         currentTag=leftoverText.Substring(currentOpenTag,currentCloseTag+1);
        //         currentTagIndex=leftoverText.IndexOf(currentTag)+currentTag.Count();
        //         mirrorEndTag=currentTag.Substring(0, 1) + "/" + currentTag.Substring(1);
        //         mirrorEndTagIndex=leftoverText.LastIndexOf(mirrorEndTag);
        //         if(mirrorEndTagIndex>0&&currentTag!="<>"){
        //             leftoverText=leftoverText.Substring(currentTagIndex,mirrorEndTagIndex-mirrorEndTag.Count()+1);
        //             if(leftoverText.IndexOf('<')>-1){
        //                 leftoverText=FindTag(leftoverText);
        //             }
        //         }
        //         else{
        //             leftoverText="";
        //         }
        //     }
        // }
    //     return leftoverText;
    // }
        private List<FoundLetter> currentWordLettersPosition { get; set; }
        private List<List<FoundLetter>> wordsSolved { get; set; }
        public IActionResult Index()
        {
//             var stringJava=new List<string>(){"<h1>some</h1>",
// "<h1>had<h1>public</h1></h1>",
// "<h1>had<h1>public</h1515></h1>",
// "<h1><h1></h1></h1>",
// "<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<",
// ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>",
// "<<<<<<<<<<<<<<<<<<<<<>>>>>>>>>>>>>>>>>>>>>>>>>><<<<<<<<<<<<<<<<<<<<<<<<<<<>>>>>>>>>>>>>>>>>>>>>>>",
// "<>hello</>",
// "<>hello</><h>dim</h>",
// "<>hello</><h>dim</h>>>>>"};
//             foreach (var line in stringJava)
//             {
//                 var leftoverText=FindTag(line);
//                 if(leftoverText==""){
//                    leftoverText="none";
//                 }
//                 else{

//                 }
//             }


            var directory=System.IO.Directory.GetCurrentDirectory()+"//Docs//";
            var cypher =JsonConvert.DeserializeObject<List<string>>(System.IO.File.ReadAllText(directory+"cypher.json"));
            var baseDoc =JsonConvert.DeserializeObject<List<Base>>(System.IO.File.ReadAllText(directory+"base.json"));
            var values =JsonConvert.DeserializeObject<List<List<Value>>>(System.IO.File.ReadAllText(directory+"values.json"));
            var words =JsonConvert.DeserializeObject<List<string>>(System.IO.File.ReadAllText(directory+"words.json"));
            var matrixReturn=new List<RowModel>();
            for (int indexCypher = 0; indexCypher < cypher.Count; indexCypher++)
            {
                matrixReturn.Add(RowBuilder(indexCypher,cypher.ElementAt(indexCypher),values.ElementAt(indexCypher),baseDoc));
            }
            matrixReturn=PuzzleSolver(matrixReturn,words);
            // var words =JsonConvert.DeserializeObject(System.IO.File.ReadAllText(directory+"words.json"));
            return View(matrixReturn);
        }

        private RowModel RowBuilder(int rowNumber,string rawString, List<Value> rowUseRules, List<Base> rules){
            string decriptedResult=rawString;
            foreach (Value rowUseRule in rowUseRules.OrderByDescending(x=>x.Order))
            {
               Base rule=rules.ElementAt(rowUseRule.Rule);
               decriptedResult=decriptedResult.Replace(rule.Source,rule.Replacement);
            }
            var chartArray=decriptedResult.ToCharArray();
            var rowResult= new RowModel(){
                Row=rowNumber,
                Letters=new List<LetterPositionModel>()
            };
            for (int column = 0; column < chartArray.Count(); column++)
            {  
                var currentLetter=new LetterPositionModel(){
                    Character=chartArray.ElementAt(column),
                    Column=column,
                    isDifferentColor=false
                };
                rowResult.Letters.Add(currentLetter);
            }
            return rowResult;
        }

        private List<RowModel> PuzzleSolver(List<RowModel> stringRows, List<string> words){
            wordsSolved=new List<List<FoundLetter>>();
            foreach (string word in words)
            {
                currentWordLettersPosition=new List<FoundLetter>();
                var charWord=word.ToCharArray();
                var currentWordLetter=charWord.ElementAt(0);
                for (int rowIndex = 0; rowIndex < stringRows.Count; rowIndex++)
                {
                    var currentRow=stringRows.ElementAt(rowIndex);
                    for (int currentRowLetter = 0; currentRowLetter < currentRow.Letters.Count; currentRowLetter++)
                    {
                        var hitForTrue=currentWordLetter==currentRow.Letters.ElementAt(currentRowLetter).Character;
                        if(hitForTrue){
                            currentWordLettersPosition.Add(new FoundLetter(rowIndex,currentRowLetter));
                            SquareFinder(stringRows,charWord,1,currentRowLetter,rowIndex);
                            currentWordLettersPosition=new List<FoundLetter>();
                        }
                    }
                    
                }
            }
            var solvedStringRows=WordsPainter(stringRows);
            return solvedStringRows;
        }

        private List<RowModel> WordsPainter(List<RowModel> stringRows){
            foreach (var wordFinded in wordsSolved)
            {
                foreach (var letter in wordFinded)
                {  
                    stringRows.FirstOrDefault(x=>x.Row==letter.Row).Letters.ElementAt(letter.Column).isDifferentColor=true;
                }
            }
            return stringRows;
        }
        private void SquareFinder(List<RowModel> stringRows,char[] charWord,int nextLetterIndex,int currentXPosition,int currentYPosition){
            Founder(stringRows,charWord,nextLetterIndex,currentXPosition+1,currentYPosition);
            Founder(stringRows,charWord,nextLetterIndex,currentXPosition-1,currentYPosition);
            Founder(stringRows,charWord,nextLetterIndex,currentXPosition,currentYPosition+1);
            Founder(stringRows,charWord,nextLetterIndex,currentXPosition,currentYPosition-1);
            Founder(stringRows,charWord,nextLetterIndex,currentXPosition+1,currentYPosition+1);
            Founder(stringRows,charWord,nextLetterIndex,currentXPosition+1,currentYPosition-1);
            Founder(stringRows,charWord,nextLetterIndex,currentXPosition-1,currentYPosition*1);
            Founder(stringRows,charWord,nextLetterIndex,currentXPosition-1,currentYPosition-1);
        }
        private void Founder(List<RowModel> stringRows,char[] charWord,int nextLetterIndex,int currentXPosition,int currentYPosition){
            var hitForTrue=false;
            var currentRow=stringRows.FirstOrDefault(x=>x.Row==currentYPosition);
            if(currentRow!=null){
                var columnInRow=currentRow.Letters.FirstOrDefault(x=>x.Column==currentXPosition);
                if(columnInRow!=null){
                    hitForTrue=columnInRow.Character.CompareTo(charWord.ElementAt(nextLetterIndex))==0;
                    if(hitForTrue){
                        currentWordLettersPosition.Add(new FoundLetter(currentYPosition,currentXPosition));
                        if(nextLetterIndex+1<charWord.Count()){
                            SquareFinder(stringRows,charWord,nextLetterIndex+1,currentXPosition,currentYPosition);
                        }
                        else{
                            wordsSolved.Add(currentWordLettersPosition);
                        }
                    }
                }
            }
        }
        
    }
}
