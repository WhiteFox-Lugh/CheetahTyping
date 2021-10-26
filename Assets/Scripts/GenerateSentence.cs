using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

public class GenerateSentence
{
  private const int minLength = 1;
  private const int maxLength = 50;
  // AssetBundle
  private static AssetBundle abData;
  private const string ABPathLocal = "AssetBundleData/wordset_short";
  private const string ABPath = "https://whitefox-lugh.github.io/FoxTyping/AssetBundleData/wordset_short";
  private static int lang;
  private static Dictionary<string, int> langMap = new Dictionary<string, int> {
    {"Japanese", 0},
    {"English", 1},
  };

  // ひらがな -> ローマ字マッピング
  private static Dictionary<string, string[]> romanTypeMap = new Dictionary<string, string[]> {
    {"あ", new string[1] {"a"}},
    {"い", new string[2] {"i", "yi"}},
    {"う", new string[3] {"u", "wu", "whu"}},
    {"え", new string[1] {"e"}},
    {"お", new string[1] {"o"}},
    {"か", new string[2] {"ka", "ca"}},
    {"き", new string[1] {"ki"}},
    {"く", new string[3] {"ku", "cu", "qu"}},
    {"け", new string[1] {"ke"}},
    {"こ", new string[2] {"ko", "co"}},
    {"さ", new string[1] {"sa"}},
    {"し", new string[3] {"si", "shi", "ci"}},
    {"す", new string[1] {"su"}},
    {"せ", new string[2] {"se", "ce"}},
    {"そ", new string[1] {"so"}},
    {"た", new string[1] {"ta"}},
    {"ち", new string[2] {"ti", "chi"}},
    {"つ", new string[2] {"tu", "tsu"}},
    {"て", new string[1] {"te"}},
    {"と", new string[1] {"to"}},
    {"な", new string[1] {"na"}},
    {"に", new string[1] {"ni"}},
    {"ぬ", new string[1] {"nu"}},
    {"ね", new string[1] {"ne"}},
    {"の", new string[1] {"no"}},
    {"は", new string[1] {"ha"}},
    {"ひ", new string[1] {"hi"}},
    {"ふ", new string[2] {"fu", "hu"}},
    {"へ", new string[1] {"he"}},
    {"ほ", new string[1] {"ho"}},
    {"ま", new string[1] {"ma"}},
    {"み", new string[1] {"mi"}},
    {"む", new string[1] {"mu"}},
    {"め", new string[1] {"me"}},
    {"も", new string[1] {"mo"}},
    {"や", new string[1] {"ya"}},
    {"ゆ", new string[1] {"yu"}},
    {"よ", new string[1] {"yo"}},
    {"ら", new string[1] {"ra"}},
    {"り", new string[1] {"ri"}},
    {"る", new string[1] {"ru"}},
    {"れ", new string[1] {"re"}},
    {"ろ", new string[1] {"ro"}},
    {"わ", new string[1] {"wa"}},
    {"を", new string[1] {"wo"}},
    {"ん", new string[3] {"n", "nn", "xn"}},
    {"が", new string[1] {"ga"}},
    {"ぎ", new string[1] {"gi"}},
    {"ぐ", new string[1] {"gu"}},
    {"げ", new string[1] {"ge"}},
    {"ご", new string[1] {"go"}},
    {"ざ", new string[1] {"za"}},
    {"じ", new string[2] {"ji", "zi"}},
    {"ず", new string[1] {"zu"}},
    {"ぜ", new string[1] {"ze"}},
    {"ぞ", new string[1] {"zo"}},
    {"だ", new string[1] {"da"}},
    {"ぢ", new string[1] {"di"}},
    {"づ", new string[1] {"du"}},
    {"で", new string[1] {"de"}},
    {"ど", new string[1] {"do"}},
    {"ば", new string[1] {"ba"}},
    {"び", new string[1] {"bi"}},
    {"ぶ", new string[1] {"bu"}},
    {"べ", new string[1] {"be"}},
    {"ぼ", new string[1] {"bo"}},
    {"ぱ", new string[1] {"pa"}},
    {"ぴ", new string[1] {"pi"}},
    {"ぷ", new string[1] {"pu"}},
    {"ぺ", new string[1] {"pe"}},
    {"ぽ", new string[1] {"po"}},
    {"ぁ", new string[2] {"la", "xa"}},
    {"ぃ", new string[4] {"li", "xi", "lyi", "xyi"}},
    {"ぅ", new string[2] {"lu", "xu"}},
    {"ぇ", new string[4] {"le", "xe", "lye", "xye"}},
    {"ぉ", new string[2] {"lo", "xo"}},
    {"ゃ", new string[2] {"lya", "xya"}},
    {"ゅ", new string[2] {"lyu", "xyu"}},
    {"ょ", new string[2] {"lyo", "xyo"}},
    {"っ", new string[4] {"ltu", "ltsu", "xtu", "xtsu"}},
    {"きゃ", new string[3] {"kya", "kilya", "kixya"}},
    {"きぃ", new string[5] {"kyi", "kili", "kilyi", "kixi", "kixyi"}},
    {"きゅ", new string[3] {"kyu", "kilyu", "kixyu"}},
    {"きぇ", new string[5] {"kye", "kile", "kilye", "kixe", "kixye"}},
    {"きょ", new string[3] {"kyo", "kilyo", "kixyo"}},
    {"ぎゃ", new string[3] {"gya", "gilya", "gixya"}},
    {"ぎぃ", new string[5] {"gyi", "gili", "gilyi", "gixi", "gixyi"}},
    {"ぎゅ", new string[3] {"gyu", "gilyu", "gixyu"}},
    {"ぎぇ", new string[5] {"gye", "gile", "gilye", "gixe", "gixye"}},
    {"ぎょ", new string[3] {"gyo", "gilyo", "gixyo"}},
    {"くぁ", new string[8] {"qa", "kwa", "kula", "kuxa", "cula", "cuxa", "qula", "quxa"}},
    {"くぃ", new string[14] {"qi", "qyi", "kuli", "kuxi", "kulyi", "kuxyi", "culi", "culyi", "cuxi", "cuxyi", "quli", "quxi", "qulyi", "quxyi"}},
    {"くぅ", new string[7] {"qwu", "kulu", "kuxu", "culu", "cuxu", "qulu", "quxu"}},
    {"くぇ", new string[14] {"qe", "qwe", "kule", "kuxe", "kulye", "kuxye", "cule", "cuxe", "culye", "cuxye", "qule", "quxe", "qulye", "quxye"}},
    {"くぉ", new string[8] {"qo", "qwo", "kulo", "kuxo", "culo", "cuxo", "qulo", "quxo"}},
    {"ぐぁ", new string[3] {"gwa", "gula", "guxa"}},
    {"ぐぃ", new string[5] {"gwi", "guli", "gulyi", "guxi", "guxyi"}},
    {"ぐぅ", new string[3] {"gwu", "gulu", "guxu"}},
    {"ぐぇ", new string[5] {"gwe", "gule", "guxe", "gulye", "guxye"}},
    {"ぐぉ", new string[3] {"gwo", "gulo", "guxo"}},
    {"しゃ", new string[8] {"sya", "sha", "silya", "sixya", "shilya", "shixya", "cilya", "cixya"}},
    {"しぃ", new string[13] {"syi", "sili", "sixi", "silyi", "sixyi", "shili", "shixi", "shilyi", "shixyi", "cili", "cixi", "cilyi", "cixyi"}},
    {"しゅ", new string[8] {"syu", "shu", "silyu", "sixyu", "shilyu", "shixyu", "cilyu", "cixyu"}},
    {"しぇ", new string[14] {"sye", "she", "sile", "silye", "sixe", "sixye", "shile", "shilye", "shixe", "shixye", "cile", "cilye", "cixe", "cixye"}},
    {"しょ", new string[8] {"syo", "sho", "silyo", "sixyo", "shilyo", "shixyo", "cilyo", "cixyo"}},
    {"じゃ", new string[7] {"ja", "jya", "zya", "jilya", "jixya", "zilya", "zixya"}},
    {"じぃ", new string[10] {"jyi", "zyi", "jili", "jixi", "jilyi", "jixyi", "zili", "zixi", "zilyi", "zixyi"}},
    {"じゅ", new string[7] {"ju", "jyu", "zyu", "jilyu", "jixyu", "zilyu", "zixyu"}},
    {"じぇ", new string[11] {"je", "jye", "zye", "jile", "jixe", "jilye", "jixye", "zile", "zixe", "zilye", "zixye"}},
    {"じょ", new string[7] {"jo", "jyo", "zyo", "jilyo", "jixyo", "zilyo", "zixyo"}},
    {"すぁ", new string[3] {"swa", "sula", "suxa"}},
    {"すぃ", new string[5] {"swi", "suli", "sulyi", "suxi", "suxyi"}},
    {"すぅ", new string[3] {"swu", "sulu", "suxu"}},
    {"すぇ", new string[5] {"swe", "sule", "suxe", "sulye", "suxye"}},
    {"すぉ", new string[3] {"swo", "sulo", "suxo"}},
    {"ちゃ", new string[7] {"tya", "cha", "cya", "tilya", "tixya", "chilya", "chixya"}},
    {"ちぃ", new string[10] {"tyi", "cyi", "tili", "tixi", "tilyi", "tixyi", "chili", "chixi", "chilyi", "chixyi"}},
    {"ちゅ", new string[7] {"tyu", "chu", "cyu", "tilyu", "tixyu", "chilyu", "chixyu"}},
    {"ちぇ", new string[11] {"tye", "che", "cye", "tile", "tixe", "tilye", "tixye", "chile", "chixe", "chilye", "chixye"}},
    {"ちょ", new string[7] {"tyo", "cho", "cyo", "tilyo", "tixyo", "chilyo", "chixyo"}},
    {"ぢゃ", new string[3] {"dya", "dilya", "dixya"}},
    {"ぢゅ", new string[3] {"dyu", "dilyu", "dixyu"}},
    {"ぢぇ", new string[5] {"dye", "dile", "dixe", "dilye", "dixye"}},
    {"ぢょ", new string[3] {"dyo", "dilyo", "dixyo"}},
    {"つぁ", new string[5] {"tsa", "tula", "tuxa", "tsula", "tsuxa"}},
    {"つぃ", new string[9] {"tsi", "tuli", "tuxi", "tulyi", "tuxyi", "tsuli", "tsuxi", "tsulyi", "tsuxyi"}},
    {"つぇ", new string[9] {"tse", "tule", "tuxe", "tulye", "tuxye", "tsule", "tsuxe", "tsulye", "tsuxye"}},
    {"つぉ", new string[5] {"tso", "tulo", "tuxo", "tsulo", "tsuxo"}},
    {"てゃ", new string[3] {"tha", "telya", "texya"}},
    {"てぃ", new string[5] {"thi", "teli", "texi", "telyi", "texyi"}},
    {"てゅ", new string[3] {"thu", "telyu", "texyu"}},
    {"てぇ", new string[5] {"the", "tele", "texe", "telye", "texye"}},
    {"てょ", new string[3] {"tho", "telyo", "texyo"}},
    {"でゃ", new string[3] {"dha", "delya", "dexya"}},
    {"でぃ", new string[5] {"dhi", "deli", "dexi", "delyi", "dexyi"}},
    {"でゅ", new string[3] {"dhu", "delyu", "dexyu"}},
    {"でぇ", new string[5] {"dhe", "dele", "dexe", "delye", "dexye"}},
    {"でょ", new string[3] {"dho", "delyo", "dexyo"}},
    {"とぁ", new string[3] {"twa", "tola", "toxa"}},
    {"とぃ", new string[5] {"twi", "toli", "toxi", "tolyi", "toxyi"}},
    {"とぅ", new string[3] {"twu", "tolu", "toxu"}},
    {"とぇ", new string[5] {"twe", "tole", "toxe", "tolye", "toxye"}},
    {"とぉ", new string[3] {"two", "tolo", "toxo"}},
    {"どぁ", new string[3] {"dwa", "dola", "doxa"}},
    {"どぃ", new string[5] {"dwi", "doli", "doxi", "dolyi", "doxyi"}},
    {"どぅ", new string[3] {"dwu", "dolu", "doxu"}},
    {"どぇ", new string[5] {"dwe", "dole", "doxe", "dolye", "doxye"}},
    {"どぉ", new string[3] {"dwo", "dolo", "doxo"}},
    {"にゃ", new string[3] {"nya", "nilya", "nixya"}},
    {"にぃ", new string[5] {"nyi", "nili", "nixi", "nilyi", "nixyi"}},
    {"にゅ", new string[3] {"nyu", "nilyu", "nixyu"}},
    {"にぇ", new string[5] {"nye", "nile", "nixe", "nilye", "nixye"}},
    {"にょ", new string[3] {"nyo", "nilyo", "nixyo"}},
    {"ぴゃ", new string[3] {"pya", "pilya", "pixya"}},
    {"ぴぃ", new string[5] {"pyi", "pili", "pixi", "pilyi", "pixyi"}},
    {"ぴゅ", new string[3] {"pyu", "pilyu", "pixyu"}},
    {"ぴぇ", new string[5] {"pye", "pile", "pixe", "pilye", "pixye"}},
    {"ぴょ", new string[3] {"pyo", "pilyo", "pixyo"}},
    {"ひゃ", new string[3] {"hya", "hilya", "hixya"}},
    {"ひぃ", new string[5] {"hyi", "hili", "hixi", "hilyi", "hixyi"}},
    {"ひゅ", new string[3] {"hyu", "hilyu", "hixyu"}},
    {"ひぇ", new string[5] {"hye", "hile", "hixe", "hilye", "hixye"}},
    {"ひょ", new string[3] {"hyo", "hilyo", "hixyo"}},
    {"びゃ", new string[3] {"bya", "bilya", "bixya"}},
    {"びぃ", new string[5] {"byi", "bili", "bixi", "bilyi", "bixyi"}},
    {"びゅ", new string[3] {"byu", "bilyu", "bixyu"}},
    {"びぇ", new string[5] {"bye", "bile", "bixe", "bilye", "bixye"}},
    {"びょ", new string[3] {"byo", "bilyo", "bixyo"}},
    {"ふゃ", new string[5] {"fya", "fulya", "fuxya", "hulya", "huxya"}},
    {"ふぃ", new string[10] {"fi", "fyi", "fuli", "fuxi", "fulyi", "fuxyi", "huli", "huxi", "hulyi", "huxyi"}},
    {"ふゅ", new string[5] {"fyu", "fulyu", "fuxyu", "hulyu", "huxyu"}},
    {"ふぇ", new string[10] {"fe", "fye", "fule", "fuxe", "fulye", "fuxye", "hule", "huxe", "hulye", "huxye"}},
    {"ふょ", new string[5] {"fyo", "fulyo", "fuxyo", "hulyo", "huxyo"}},
    {"ふぁ", new string[5] {"fa", "fula", "fuxa", "hula", "huxa"}},
    {"ふぉ", new string[5] {"fo", "fulo", "fuxo", "hulo", "huxo"}},
    {"みゃ", new string[3] {"mya", "milya", "mixya"}},
    {"みぃ", new string[5] {"myi", "mili", "mixi", "milyi", "mixyi"}},
    {"みゅ", new string[3] {"myu", "milyu", "mixyu"}},
    {"みぇ", new string[5] {"mye", "mile", "mixe", "milye", "mixye"}},
    {"みょ", new string[3] {"myo", "milyo", "mixyo"}},
    {"りゃ", new string[3] {"rya", "rilya", "rixya"}},
    {"りぃ", new string[5] {"ryi", "rili", "rixi", "rilyi", "rixyi"}},
    {"りゅ", new string[3] {"ryu", "rilyu", "rixyu"}},
    {"りぇ", new string[5] {"rye", "rile", "rixe", "rilye", "rixye"}},
    {"りょ", new string[3] {"ryo", "rilyo", "rixyo"}},
    {"うぁ", new string[7] {"wha", "ula", "uxa", "wula", "wuxa", "whula", "whuxa"}},
    {"うぃ", new string[11] {"wi", "whi", "uli", "uxi", "ulyi", "uxyi", "wuli", "wuxi", "wulyi", "wuxyi", "whuli"}},
    {"うぇ", new string[14] {"we", "whe", "ule", "uxe", "ulye", "uxye", "wule", "wuxe", "wulye", "wuxye", "whule", "whuxe", "whulye", "whuxye"}},
    {"うぉ", new string[7] {"who", "ulo", "uxo", "wulo", "wuxo", "whulo", "whuxo"}},
    {"ゔぁ", new string[3] {"va", "vula", "vuxa"}},
    {"ゔぃ", new string[6] {"vi", "vyi", "vuli", "vuxi", "vulyi", "vuxyi"}},
    {"ゔ", new string[1] {"vu"}},
    {"ゔぇ", new string[6] {"ve", "vye", "vule", "vuxe", "vulye", "vuxye"}},
    {"ゔぉ", new string[3] {"vo", "vulo", "vuxo"}},
    {"ゔゃ", new string[3] {"vya", "vulya", "vuxya"}},
    {"ゔゅ", new string[3] {"vyu", "vulyu", "vuxyu"}},
    {"ゔょ", new string[3] {"vyo", "vulyo", "vuxyo"}},
    {"ゐ", new string[1] {"wyi"}},
    {"ゑ", new string[1] {"wye"}},
    {"んあ", new string[2] {"nna", "xna"}},
    {"んい", new string[4] {"nni", "xni", "nnyi", "xnyi"}},
    {"んう", new string[6] {"nnu", "xnu", "nnwu", "xnwu", "nnwhu", "xnwhu"}},
    {"んえ", new string[2] {"nne", "xne"}},
    {"んお", new string[2] {"nno", "xno"}},
    {"んか", new string[6] {"nka", "nca", "nnka", "nnca", "xnka", "xnca"}},
    {"んき", new string[3] {"nki", "nnki", "xnki"}},
    {"んく", new string[9] {"nku", "ncu", "nqu", "nnku", "nncu", "nnqu", "xnku", "xncu", "xnqu"}},
    {"んけ", new string[3] {"nke", "nnke", "xnke"}},
    {"んこ", new string[6] {"nko", "nco", "nnko", "nnco", "xnko", "xnco"}},
    {"んさ", new string[3] {"nsa", "nnsa", "xnsa"}},
    {"んし", new string[9] {"nsi", "nshi", "nci", "nnsi", "nnshi", "nnci", "xnsi", "xnshi", "xnci"}},
    {"んす", new string[3] {"nsu", "nnsu", "xnsu"}},
    {"んせ", new string[6] {"nse", "nce", "nnse", "nnce", "xnse", "xnce"}},
    {"んそ", new string[3] {"nso", "nnso", "xnso"}},
    {"んた", new string[3] {"nta", "nnta", "xnta"}},
    {"んち", new string[6] {"nti", "nchi", "nnti", "nnchi", "xnti", "xnchi"}},
    {"んつ", new string[6] {"ntu", "ntsu", "nntu", "nntsu", "xntu", "xntsu"}},
    {"んて", new string[3] {"nte", "nnte", "xnte"}},
    {"んと", new string[3] {"nto", "nnto", "xnto"}},
    {"んな", new string[2] {"nnna", "xnna"}},
    {"んに", new string[2] {"nnni", "xnni"}},
    {"んぬ", new string[2] {"nnnu", "xnnu"}},
    {"んね", new string[2] {"nnne", "xnne"}},
    {"んの", new string[2] {"nnno", "xnno"}},
    {"んは", new string[3] {"nha", "nnha", "xnha"}},
    {"んひ", new string[3] {"nhi", "nnhi", "xnhi"}},
    {"んふ", new string[6] {"nfu", "nhu", "nnfu", "nnhu", "xnfu", "xnhu"}},
    {"んへ", new string[3] {"nhe", "nnhe", "xnhe"}},
    {"んほ", new string[3] {"nho", "nnho", "xnho"}},
    {"んま", new string[3] {"nma", "nnma", "xnma"}},
    {"んみ", new string[3] {"nmi", "nnmi", "xnmi"}},
    {"んむ", new string[3] {"nmu", "nnmu", "xnmu"}},
    {"んめ", new string[3] {"nme", "nnme", "xnme"}},
    {"んも", new string[3] {"nmo", "nnmo", "xnmo"}},
    {"んや", new string[2] {"nnya", "xnya"}},
    {"んゆ", new string[2] {"nnyu", "xnyu"}},
    {"んよ", new string[2] {"nnyo", "xnyo"}},
    {"んら", new string[3] {"nra", "nnra", "xnra"}},
    {"んり", new string[3] {"nri", "nnri", "xnri"}},
    {"んる", new string[3] {"nru", "nnru", "xnru"}},
    {"んれ", new string[3] {"nre", "nnre", "xnre"}},
    {"んろ", new string[3] {"nro", "nnro", "xnro"}},
    {"んわ", new string[3] {"nwa", "nnwa", "xnwa"}},
    {"んを", new string[3] {"nwo", "nnwo", "xnwo"}},
    {"んが", new string[3] {"nga", "nnga", "xnga"}},
    {"んぎ", new string[3] {"ngi", "nngi", "xngi"}},
    {"んぐ", new string[3] {"ngu", "nngu", "xngu"}},
    {"んげ", new string[3] {"nge", "nnge", "xnge"}},
    {"んご", new string[3] {"ngo", "nngo", "xngo"}},
    {"んざ", new string[3] {"nza", "nnza", "xnza"}},
    {"んじ", new string[6] {"nji", "nzi", "nnji", "nnzi", "xnji", "xnzi"}},
    {"んず", new string[3] {"nzu", "nnzu", "xnzu"}},
    {"んぜ", new string[3] {"nze", "nnze", "xnze"}},
    {"んぞ", new string[3] {"nzo", "nnzo", "xnze"}},
    {"んだ", new string[3] {"nda", "nnda", "xnda"}},
    {"んぢ", new string[3] {"ndi", "nndi", "xndi"}},
    {"んづ", new string[3] {"ndu", "nndu", "xndu"}},
    {"んで", new string[3] {"nde", "nnde", "xnde"}},
    {"んど", new string[3] {"ndo", "nndo", "xndo"}},
    {"んば", new string[3] {"nba", "nnba", "xnba"}},
    {"んび", new string[3] {"nbi", "nnbi", "xnbi"}},
    {"んぶ", new string[3] {"nbu", "nnbu", "xnbu"}},
    {"んべ", new string[3] {"nbe", "nnbe", "xnbe"}},
    {"んぼ", new string[3] {"nbo", "nnbo", "xnbo"}},
    {"んぱ", new string[3] {"npa", "nnpa", "xnpa"}},
    {"んぴ", new string[3] {"npi", "nnpi", "xnpi"}},
    {"んぷ", new string[3] {"npu", "nnpu", "xnpu"}},
    {"んぺ", new string[3] {"npe", "nnpe", "xnpe"}},
    {"んぽ", new string[3] {"npo", "nnpo", "xnpo"}},
    {"んきゃ", new string[9] {"nkya", "nkilya", "nkixya", "nnkya", "nnkilya", "nnkixya", "xnkya", "xnkilya", "xnkixya"}},
    {"んきぃ", new string[15] {"nkyi", "nkili", "nkilyi", "nkixi", "nkixyi", "nnkyi", "nnkili", "nnkilyi", "nnkixi", "nnkixyi", "xnkyi", "xnkili", "xnkilyi", "xnkixi", "xnkixyi"}},
    {"んきゅ", new string[9] {"nkyu", "nkilyu", "nkixyu", "nnkyu", "nnkilyu", "nnkixyu", "xnkyu", "xnkilyu", "xnkixyu"}},
    {"んきぇ", new string[15] {"nkye", "nkile", "nkilye", "nkixe", "nkixye", "nnkye", "nnkile", "nnkilye", "nnkixe", "nnkixye", "xnkye", "xnkile", "xnkilye", "xnkixe", "xnkixye"}},
    {"んきょ", new string[9] {"nkyo", "nkilyo", "nkixyo", "nnkyo", "nnkilyo", "nnkixyo", "xnkyo", "xnkilyo", "xnkixyo"}},
    {"んぎゃ", new string[9] {"ngya", "ngilya", "ngixya", "nngya", "nngilya", "nngixya", "xngya", "xngilya", "xngixya"}},
    {"んぎぃ", new string[15] {"ngyi", "ngili", "ngilyi", "ngixi", "ngixyi", "nngyi", "nngili", "nngilyi", "nngixi", "nngixyi", "xngyi", "xngili", "xngilyi", "xngixi", "xngixyi"}},
    {"んぎゅ", new string[9] {"ngyu", "ngilyu", "ngixyu", "nngyu", "nngilyu", "nngixyu", "xngyu", "xngilyu", "xngixyu"}},
    {"んぎぇ", new string[15] {"ngye", "ngile", "ngilye", "ngixe", "ngixye", "nngye", "nngile", "nngilye", "nngixe", "nngixye", "xngye", "xngile", "xngilye", "xngixe", "xngixye"}},
    {"んぎょ", new string[9] {"ngyo", "ngilyo", "ngixyo", "nngyo", "nngilyo", "nngixyo", "xngyo", "xngilyo", "xngixyo"}},
    {"んしゃ", new string[24] {"nsya", "nsha", "nsilya", "nsixya", "nshilya", "nshixya", "ncilya", "ncixya",
                              "nnsya", "nnsha", "nnsilya", "nnsixya", "nnshilya", "nnshixya", "nncilya", "nncixya",
                              "xnsya", "xnsha", "xnsilya", "xnsixya", "xnshilya", "xnshixya", "xncilya", "xncixya"}},
    {"んしぃ", new string[39] {"nsyi", "nsili", "nsixi", "nsilyi", "nsixyi", "nshili", "nshixi", "nshilyi", "nshixyi", "ncili", "ncixi", "ncilyi", "ncixyi",
                              "nnsyi", "nnsili", "nnsixi", "nnsilyi", "nnsixyi", "nnshili", "nnshixi", "nnshilyi", "nnshixyi", "nncili", "nncixi", "nncilyi", "nncixyi",
                              "xnsyi", "xnsili", "xnsixi", "xnsilyi", "xnsixyi", "xnshili", "xnshixi", "xnshilyi", "xnshixyi", "xncili", "xncixi", "xncilyi", "xncixyi"}},
    {"んしゅ", new string[24] {"nsyu", "nshu", "nsilyu", "nsixyu", "nshilyu", "nshixyu", "ncilyu", "ncixyu",
                              "nnsyu", "nnshu", "nnsilyu", "nnsixyu", "nnshilyu", "nnshixyu", "nncilyu", "nncixyu",
                              "xnsyu", "xnshu", "xnsilyu", "xnsixyu", "xnshilyu", "xnshixyu", "xncilyu", "xncixyu"}},
    {"んしぇ", new string[42] {"nsye", "nshe", "nsile", "nsilye", "nsixe", "nsixye", "nshile", "nshilye", "nshixe", "nshixye", "ncile", "ncilye", "ncixe", "ncixye",
                              "nnsye", "nnshe", "nnsile", "nnsilye", "nnsixe", "nnsixye", "nnshile", "nnshilye", "nnshixe", "nnshixye", "nncile", "nncilye", "nncixe", "nncixye",
                              "xnsye", "xnshe", "xnsile", "xnsilye", "xnsixe", "xnsixye", "xnshile", "xnshilye", "xnshixe", "xnshixye", "xncile", "xncilye", "xncixe", "xncixye"}},
    {"んしょ", new string[24] {"nsyo", "nsho", "nsilyo", "nsixyo", "nshilyo", "nshixyo", "ncilyo", "ncixyo",
                              "nnsyo", "nnsho", "nnsilyo", "nnsixyo", "nnshilyo", "nnshixyo", "nncilyo", "nncixyo",
                              "xnsyo", "xnsho", "xnsilyo", "xnsixyo", "xnshilyo", "xnshixyo", "xncilyo", "xncixyo"}},
    {"んじゃ", new string[21] {"nja", "njya", "nzya", "njilya", "njixya", "nzilya", "nzixya",
                              "nnja", "nnjya", "nnzya", "nnjilya", "nnjixya", "nnzilya", "nnzixya",
                              "xnja", "xnjya", "xnzya", "xnjilya", "xnjixya", "xnzilya", "xnzixya"}},
    {"んじぃ", new string[30] {"njyi", "nzyi", "njili", "njixi", "njilyi", "njixyi", "nzili", "nzixi", "nzilyi", "nzixyi",
                              "nnjyi", "nnzyi", "nnjili", "nnjixi", "nnjilyi", "nnjixyi", "nnzili", "nnzixi", "nnzilyi", "nnzixyi",
                              "xnjyi", "xnzyi", "xnjili", "xnjixi", "xnjilyi", "xnjixyi", "xnzili", "xnzixi", "xnzilyi", "xnzixyi"}},
    {"んじゅ", new string[21] {"nju", "njyu", "nzyu", "njilyu", "njixyu", "nzilyu", "nzixyu",
                              "nnju", "nnjyu", "nnzyu", "nnjilyu", "nnjixyu", "nnzilyu", "nnzixyu",
                              "xnju", "xnjyu", "xnzyu", "xnjilyu", "xnjixyu", "xnzilyu", "xnzixyu"}},
    {"んじぇ", new string[33] {"nje", "njye", "nzye", "njile", "njixe", "njilye", "njixye", "nzile", "nzixe", "nzilye", "nzixye",
                              "nnje", "nnjye", "nnzye", "nnjile", "nnjixe", "nnjilye", "nnjixye", "nnzile", "nnzixe", "nnzilye", "nnzixye",
                              "xnje", "xnjye", "xnzye", "xnjile", "xnjixe", "xnjilye", "xnjixye", "xnzile", "xnzixe", "xnzilye", "xnzixye"}},
    {"んじょ", new string[21] {"njo", "njyo", "nzyo", "njilyo", "njixyo", "nzilyo", "nzixyo",
                              "nnjo", "nnjyo", "nnzyo", "nnjilyo", "nnjixyo", "nnzilyo", "nnzixyo",
                              "xnjo", "xnjyo", "xnzyo", "xnjilyo", "xnjixyo", "xnzilyo", "xnzixyo"}},
    {"んちゃ", new string[21] {"ntya", "ncha", "ncya", "ntilya", "ntixya", "nchilya", "nchixya",
                              "nntya", "nncha", "nncya", "nntilya", "nntixya", "nnchilya", "nnchixya",
                              "xntya", "xncha", "xncya", "xntilya", "xntixya", "xnchilya", "xnchixya"}},
    {"んちぃ", new string[30] {"ntyi", "ncyi", "ntili", "ntixi", "ntilyi", "ntixyi", "nchili", "nchixi", "nchilyi", "nchixyi",
                              "nntyi", "nncyi", "nntili", "nntixi", "nntilyi", "nntixyi", "nnchili", "nnchixi", "nnchilyi", "nnchixyi",
                              "xntyi", "xncyi", "xntili", "xntixi", "xntilyi", "xntixyi", "xnchili", "xnchixi", "xnchilyi", "xnchixyi"}},
    {"んちゅ", new string[21] {"ntyu", "nchu", "ncyu", "ntilyu", "ntixyu", "nchilyu", "nchixyu",
                              "nntyu", "nnchu", "nncyu", "nntilyu", "nntixyu", "nnchilyu", "nnchixyu",
                              "xntyu", "xnchu", "xncyu", "xntilyu", "xntixyu", "xnchilyu", "xnchixyu"}},
    {"んちぇ", new string[33] {"ntye", "nche", "ncye", "ntile", "ntixe", "ntilye", "ntixye", "nchile", "nchixe", "nchilye", "nchixye",
                              "nntye", "nnche", "nncye", "nntile", "nntixe", "nntilye", "nntixye", "nnchile", "nnchixe", "nnhilye", "nnchixye",
                              "xntye", "xnche", "xncye", "xntile", "xntixe", "xntilye", "xntixye", "xnchile", "xnchixe", "xnchilye", "xnchixye"}},
    {"んちょ", new string[21] {"ntyo", "ncho", "ncyo", "ntilyo", "ntixyo", "nchilyo", "nchixyo",
                              "nntyo", "nncho", "nncyo", "nntilyo", "nntixyo", "nnchilyo", "nnchixyo",
                              "xntyo", "xncho", "xncyo", "xntilyo", "xntixyo", "xnchilyo", "xnchixyo"}},
    {"んぢゃ", new string[9] {"ndya", "ndilya", "ndixya", "nndya", "nndilya", "nndixya", "xndya", "xndilya", "xndixya"}},
    {"んぢゅ", new string[9] {"ndyu", "ndilyu", "ndixyu", "nndyu", "nndilyu", "nndixyu", "xndyu", "xndilyu", "xndixyu"}},
    {"んぢぇ", new string[15] {"ndye", "ndile", "ndixe", "ndilye", "ndixye", "nndye", "nndile", "nndixe", "nndilye", "nndixye","xndye", "xndile", "xndixe", "xndilye", "xndixye"}},
    {"んぢょ", new string[9] {"ndyo", "ndilyo", "ndixyo", "nndyo", "nndilyo", "nndixyo", "xndyo", "xndilyo", "xndixyo"}},
    {"んつぁ", new string[15] {"ntsa", "ntula", "ntuxa", "ntsula", "ntsuxa", "nntsa", "nntula", "nntuxa", "nntsula", "nntsuxa", "xntsa", "xntula", "xntuxa", "xntsula", "xntsuxa"}},
    {"んつぃ", new string[27] {"ntsi", "ntuli", "ntuxi", "ntulyi", "ntuxyi", "ntsuli", "ntsuxi", "ntsulyi", "ntsuxyi",
                              "nntsi", "nntuli", "nntuxi", "nntulyi", "nntuxyi", "nntsuli", "nntsuxi", "nntsulyi", "nntsuxyi",
                              "xntsi", "xntuli", "xntuxi", "xntulyi", "xntuxyi", "xntsuli", "xntsuxi", "xntsulyi", "xntsuxyi"}},
    {"んつぇ", new string[27] {"ntse", "ntule", "ntuxe", "ntulye", "ntuxye", "ntsule", "ntsuxe", "ntsulye", "ntsuxye",
                              "nntse", "nntule", "nntuxe", "nntulye", "nntuxye", "nntsule", "nntsuxe", "nntsulye", "nntsuxye",
                              "xntse", "xntule", "xntuxe", "xntulye", "xntuxye", "xntsule", "xntsuxe", "xntsulye", "xntsuxye"}},
    {"んつぉ", new string[15] {"ntso", "ntulo", "ntuxo", "ntsulo", "ntsuxo", "nntso", "nntulo", "nntuxo", "nntsulo", "nntsuxo", "xntso", "xntulo", "xntuxo", "xntsulo", "xntsuxo"}},
    {"んてゃ", new string[9] {"ntha", "ntelya", "ntexya", "nntha", "nntelya", "nntexya", "xntha", "xntelya", "xntexya"}},
    {"んてぃ", new string[15] {"nthi", "nteli", "ntexi", "ntelyi", "ntexyi", "nnthi", "nnteli", "nntexi", "nntelyi", "nntexyi", "xnthi", "xnteli", "xntexi", "xntelyi", "xntexyi"}},
    {"んてゅ", new string[9] {"nthu", "ntelyu", "ntexyu", "nnthu", "nntelyu", "nntexyu", "xnthu", "xntelyu", "xntexyu"}},
    {"んてぇ", new string[15] {"nthe", "ntele", "ntexe", "ntelye", "ntexye", "nnthe", "nntele", "nntexe", "nntelye", "nntexye", "xnthe", "xntele", "xntexe", "xntelye", "xntexye"}},
    {"んてょ", new string[9] {"ntho", "ntelyo", "ntexyo", "nntho", "nntelyo", "nntexyo", "xntho", "xntelyo", "xntexyo"}},
    {"んでゃ", new string[9] {"ndha", "ndelya", "ndexya", "nndha", "nndelya", "nndexya", "xndha", "xndelya", "xndexya"}},
    {"んでぃ", new string[15] {"ndhi", "ndeli", "ndexi", "ndelyi", "ndexyi", "nndhi", "nndeli", "nndexi", "nndelyi", "nndexyi", "xndhi", "xndeli", "xndexi", "xndelyi", "xndexyi"}},
    {"んでゅ", new string[9] {"ndhu", "ndelyu", "ndexyu", "nndhu", "nndelyu", "nndexyu", "xndhu", "xndelyu", "xndexyu"}},
    {"んでぇ", new string[15] {"ndhe", "ndele", "ndexe", "ndelye", "ndexye", "nndhe", "nndele", "nndexe", "nndelye", "nndexye", "xndhe", "xndele", "xndexe", "xndelye", "xndexye"}},
    {"んでょ", new string[9] {"ndho", "ndelyo", "ndexyo", "nndho", "nndelyo", "nndexyo", "xndho", "xndelyo", "xndexyo"}},
    {"んとぁ", new string[9] {"ntwa", "ntola", "ntoxa", "nntwa", "nntola", "nntoxa", "xntwa", "xntola", "xntoxa"}},
    {"んとぃ", new string[15] {"ntwi", "ntoli", "ntoxi", "ntolyi", "ntoxyi", "nntwi", "nntoli", "nntoxi", "nntolyi", "nntoxyi", "xntwi", "xntoli", "xntoxi", "xntolyi", "xntoxyi"}},
    {"んとぅ", new string[9] {"ntwu", "ntolu", "ntoxu", "nntwu", "nntolu", "nntoxu", "xntwu", "xntolu", "xntoxu"}},
    {"んとぇ", new string[15] {"ntwe", "ntole", "ntoxe", "ntolye", "ntoxye", "nntwe", "nntole", "nntoxe", "nntolye", "nntoxye", "xntwe", "xntole", "xntoxe", "xntolye", "xntoxye"}},
    {"んとぉ", new string[9] {"ntwo", "ntolo", "ntoxo", "nntwo", "nntolo", "nntoxo", "xntwo", "xntolo", "xntoxo"}},
    {"んどぁ", new string[9] {"ndwa", "ndola", "ndoxa", "nndwa", "nndola", "nndoxa", "xndwa", "xndola", "xndoxa"}},
    {"んどぃ", new string[15] {"ndwi", "ndoli", "ndoxi", "ndolyi", "ndoxyi", "nndwi", "nndoli", "nndoxi", "nndolyi", "nndoxyi", "xndwi", "xndoli", "xndoxi", "xndolyi", "xndoxyi"}},
    {"んどぅ", new string[9] {"ndwu", "ndolu", "ndoxu", "nndwu", "nndolu", "nndoxu", "xndwu", "xndolu", "xndoxu"}},
    {"んどぇ", new string[15] {"ndwe", "ndole", "ndoxe", "ndolye", "ndoxye", "nndwe", "nndole", "nndoxe", "nndolye", "nndoxye", "xndwe", "xndole", "xndoxe", "xndolye", "xndoxye"}},
    {"んどぉ", new string[9] {"ndwo", "ndolo", "ndoxo", "nndwo", "nndolo", "nndoxo", "xndwo", "xndolo", "xndoxo"}},
    {"んにゃ", new string[6] {"nnnya", "xnnya", "nnnixya", "xnnixya", "nnnilya", "xnnilya"}},
    {"んにゅ", new string[6] {"nnnyu", "xnnyu", "nnnixyu", "xnnixyu", "nnnilyu", "xnnilyu"}},
    {"んにょ", new string[6] {"nnnyo", "xnnyo", "nnnixyo", "xnnixyo", "nnnilyo", "xnnilyo"}},
    {"んぴゃ", new string[9] {"npya", "npilya", "npixya", "nnpya", "nnpilya", "nnpixya", "xnpya", "xnpilya", "xnpixya"}},
    {"んぴぃ", new string[15] {"npyi", "npili", "npixi", "npilyi", "npixyi", "nnpyi", "nnpili", "nnpixi", "nnpilyi", "nnpixyi", "xnpyi", "xnpili", "xnpixi", "xnpilyi", "xnpixyi"}},
    {"んぴゅ", new string[9] {"npyu", "npilyu", "npixyu", "nnpyu", "nnpilyu", "nnpixyu", "xnpyu", "xnpilyu", "xnpixyu"}},
    {"んぴぇ", new string[15] {"npye", "npile", "npixe", "npilye", "npixye", "nnpye", "nnpile", "nnpixe", "nnpilye", "nnpixye", "xnpye", "xnpile", "xnpixe", "xnpilye", "xnpixye"}},
    {"んぴょ", new string[9] {"npyo", "npilyo", "npixyo", "nnpyo", "nnpilyo", "nnpixyo", "xnpyo", "xnpilyo", "xnpixyo"}},
    {"んひゃ", new string[9] {"nhya", "nhilya", "nhixya", "nnhya", "nnhilya", "nnhixya", "xnhya", "xnhilya", "xnhixya"}},
    {"んひぃ", new string[15] {"nhyi", "nhili", "nhixi", "nhilyi", "nhixyi", "nnhyi", "nnhili", "nnhixi", "nnhilyi", "nnhixyi", "xnhyi", "xnhili", "xnhixi", "xnhilyi", "xnhixyi"}},
    {"んひゅ", new string[9] {"nhyu", "nhilyu", "nhixyu", "nnhyu", "nnhilyu", "nnhixyu", "xnhyu", "xnhilyu", "xnhixyu"}},
    {"んひぇ", new string[15] {"nhye", "nhile", "nhixe", "nhilye", "nhixye", "nnhye", "nnhile", "nnhixe", "nnhilye", "nnhixye", "xnhye", "xnhile", "xnhixe", "xnhilye", "xnhixye"}},
    {"んひょ", new string[9] {"nhyo", "nhilyo", "nhixyo", "nnhyo", "nnhilyo", "nnhixyo", "xnhyo", "xnhilyo", "xnhixyo"}},
    {"んびゃ", new string[9] {"nbya", "nbilya", "nbixya", "nnbya", "nnbilya", "nnbixya", "xnbya", "xnbilya", "xnbixya"}},
    {"んびぃ", new string[15] {"nbyi", "nbili", "nbixi", "nbilyi", "nbixyi", "nnbyi", "nnbili", "nnbixi", "nnbilyi", "nnbixyi", "xnbyi", "xnbili", "xnbixi", "xnbilyi", "xnbixyi"}},
    {"んびゅ", new string[9] {"nbyu", "nbilyu", "nbixyu", "nnbyu", "nnbilyu", "nnbixyu", "xnbyu", "xnbilyu", "xnbixyu"}},
    {"んびぇ", new string[15] {"nbye", "nbile", "nbixe", "nbilye", "nbixye", "nnbye", "nnbile", "nnbixe", "nnbilye", "nnbixye", "xnbye", "xnbile", "xnbixe", "xnbilye", "xnbixye"}},
    {"んびょ", new string[9] {"nbyo", "nbilyo", "nbixyo", "nnbyo", "nnbilyo", "nnbixyo", "xnbyo", "xnbilyo", "xnbixyo"}},
    {"んふゃ", new string[15] {"nfya", "nfulya", "nfuxya", "nhulya", "nhuxya", "nnfya", "nnfulya", "nnfuxya", "nnhulya", "nnhuxya", "xnfya", "xnfulya", "xnfuxya", "xnhulya", "xnhuxya"}},
    {"んふぃ", new string[30] {"nfi", "nfyi", "nfuli", "nfuxi", "nfulyi", "nfuxyi", "nhuli", "nhuxi", "nhulyi", "nhuxyi",
                              "nnfi", "nnfyi", "nnfuli", "nnfuxi", "nnfulyi", "nnfuxyi", "nnhuli", "nnhuxi", "nnhulyi", "nnhuxyi",
                              "xnfi", "xnfyi", "xnfuli", "xnfuxi", "xnfulyi", "xnfuxyi", "xnhuli", "xnhuxi", "xnhulyi", "xnhuxyi"}},
    {"んふゅ", new string[15] {"nfyu", "nfulyu", "nfuxyu", "nhulyu", "nhuxyu", "nnfyu", "nnfulyu", "nnfuxyu", "nnhulyu", "nnhuxyu", "xnfyu", "xnfulyu", "xnfuxyu", "xnhulyu", "xnhuxyu"}},
    {"んふぇ", new string[30] {"nfe", "nfye", "nfule", "nfuxe", "nfulye", "nfuxye", "nhule", "nhuxe", "nhulye", "nhuxye",
                              "nnfe", "nnfye", "nnfule", "nnfuxe", "nnfulye", "nnfuxye", "nnhule", "nnhuxe", "nnhulye", "nnhuxye",
                              "xnfe", "xnfye", "xnfule", "xnfuxe", "xnfulye", "xnfuxye", "xnhule", "xnhuxe", "xnhulye", "xnhuxye"}},
    {"んふょ", new string[15] {"nfyo", "nfulyo", "nfuxyo", "nhulyo", "nhuxyo", "nnfyo", "nnfulyo", "nnfuxyo", "nnhulyo", "nnhuxyo", "xnfyo", "xnfulyo", "xnfuxyo", "xnhulyo", "xnhuxyo"}},
    {"んふぁ", new string[15] {"nfa", "nfula", "nfuxa", "nhula", "nhuxa", "nnfa", "nnfula", "nnfuxa", "nnhula", "nnhuxa", "xnfa", "xnfula", "xnfuxa", "xnhula", "xnhuxa"}},
    {"んふぉ", new string[15] {"nfo", "nfulo", "nfuxo", "nhulo", "nhuxo", "nnfo", "nnfulo", "nnfuxo", "nnhulo", "nnhuxo", "xnfo", "xnfulo", "xnfuxo", "xnhulo", "xnhuxo"}},
    {"んみゃ", new string[9] {"nmya", "nmilya", "nmixya", "nnmya", "nnmilya", "nnmixya", "xnmya", "xnmilya", "xnmixya"}},
    {"んみぃ", new string[15] {"nmyi", "nmili", "nmixi", "nmilyi", "nmixyi", "nnmyi", "nnmili", "nnmixi", "nnmilyi", "nnmixyi", "xnmyi", "xnmili", "xnmixi", "xnmilyi", "xnmixyi"}},
    {"んみゅ", new string[9] {"nmyu", "nmilyu", "nmixyu", "nnmyu", "nnmilyu", "nnmixyu", "xnmyu", "xnmilyu", "xnmixyu"}},
    {"んみぇ", new string[15] {"nmye", "nmile", "nmixe", "nmilye", "nmixye", "nnmye", "nnmile", "nnmixe", "nnmilye", "nnmixye", "xnmye", "xnmile", "xnmixe", "xnilye", "xnmixye"}},
    {"んみょ", new string[9] {"nmyo", "nmilyo", "nmixyo", "nnmyo", "nnmilyo", "nnmixyo", "xnmyo", "xnmilyo", "xnmixyo"}},
    {"んりゃ", new string[9] {"nrya", "nrilya", "nrixya", "nnrya", "nnrilya", "nnrixya", "xnrya", "xnrilya", "xnrixya"}},
    {"んりぃ", new string[15] {"nryi", "nrili", "nrixi", "nrilyi", "nrixyi", "nnryi", "nnrili", "nnrixi", "nnrilyi", "nnrixyi", "xnryi", "xnrili", "xnrixi", "xnrilyi", "xnrixyi"}},
    {"んりゅ", new string[9] {"nryu", "nrilyu", "nrixyu", "nnryu", "nnrilyu", "nnrixyu", "xnryu", "xnrilyu", "xnrixyu"}},
    {"んりぇ", new string[15] {"nrye", "nrile", "nrixe", "nrilye", "nrixye", "nnrye", "nnrile", "nnrixe", "nnrilye", "nnrixye", "xnrye", "xnrile", "xnrixe", "xnrilye", "xnrixye"}},
    {"んりょ", new string[9] {"nryo", "nrilyo", "nrixyo", "nnryo", "nnrilyo", "nnrixyo", "xnryo", "xnrilyo", "xnrixyo"}},
    {"んうぁ", new string[21] {"nwha", "nula", "nuxa", "nwula", "nwuxa", "nwhula", "nwhuxa",
                              "nnwha", "nnula", "nnuxa", "nnwula", "nnwuxa", "nnwhula", "nnwhuxa",
                              "xnwha", "xnula", "xnuxa", "xnwula", "xnwuxa", "xnwhula", "xnwhuxa"}},
    {"んうぃ", new string[33] {"nwi", "nwhi", "nuli", "nuxi", "nulyi", "nuxyi", "nwuli", "nwuxi", "nwulyi", "nwuxyi", "nwhuli",
                              "nnwi", "nnwhi", "nnuli", "nnuxi", "nnulyi", "nnuxyi", "nnwuli", "nnwuxi", "nnwulyi", "nnwuxyi", "nnwhuli",
                              "xnwi", "xnwhi", "xnuli", "xnuxi", "xnulyi", "xnuxyi", "xnwuli", "xnwuxi", "xnwulyi", "xnwuxyi", "xnwhuli"}},
    {"んうぇ", new string[42] {"nwe", "nwhe", "nule", "nuxe", "nulye", "nuxye", "nwule", "nwuxe", "nwulye", "nwuxye", "nwhule", "nwhuxe", "nwhulye", "nwhuxye",
                              "nnwe", "nnwhe", "nnule", "nnuxe", "nnulye", "nnuxye", "nnwule", "nnwuxe", "nnwulye", "nnwuxye", "nnwhule", "nnwhuxe", "nnwhulye", "nnwhuxye",
                              "xnwe", "xnwhe", "xnule", "xnuxe", "xnulye", "xnuxye", "xnwule", "xnwuxe", "xnwulye", "xnwuxye", "xnwhule", "xnwhuxe", "xnwhulye", "xnwhuxye"}},
    {"んうぉ", new string[21] {"nwho", "nulo", "nuxo", "nwulo", "nwuxo", "nwhulo", "nwhuxo",
                              "nnwho", "nnulo", "nnuxo", "nnwulo", "nnwuxo", "nnwhulo", "nnwhuxo",
                              "xnwho", "xnulo", "xnuxo", "xnwulo", "xnwuxo", "xnwhulo", "xnwhuxo"}},
    {"んゔぁ", new string[9] {"nva", "nvula", "nvuxa", "nnva", "nnvula", "nnvuxa", "xnva", "xnvula", "xnvuxa"}},
    {"んゔぃ", new string[18] {"nvi", "nvyi", "nvuli", "nvuxi", "nvulyi", "nvuxyi", "nnvi", "nnvyi", "nnvuli", "nnvuxi", "nnvulyi", "nnvuxyi", "xnvi", "xnvyi", "xnvuli", "xnvuxi", "xnvulyi", "xnvuxyi"}},
    {"んゔ", new string[3] {"nvu", "nnvu", "xnvu"}},
    {"んゔぇ", new string[18] {"nve", "nvye", "nvule", "nvuxe", "nvulye", "nvuxye", "nnve", "nnvye", "nnvule", "nnvuxe", "nnvulye", "nnvuxye", "xnve", "xnvye", "xnvule", "xnvuxe", "xnvulye", "xnvuxye"}},
    {"んゔぉ", new string[9] {"nvo", "nvulo", "nvuxo", "nnvo", "nnvulo", "nnvuxo", "xnvo", "xnvulo", "xnvuxo"}},
    {"んゔゃ", new string[9] {"nvya", "nvulya", "nvuxya", "nnvya", "nnvulya", "nnvuxya", "xnvya", "xnvulya", "xnvuxya"}},
    {"んゔゅ", new string[9] {"nvyu", "nvulyu", "nvuxyu", "nnvyu", "nnvulyu", "nnvuxyu", "xnvyu", "xnvulyu", "xnvuxyu"}},
    {"んゔょ", new string[9] {"nvyo", "nvulyo", "nvuxyo", "nnvyo", "nnvulyo", "nnvuxyo", "xnvyo", "xnvulyo", "xnvuxyo"}},
    {"んゐ", new string[3] {"nwyi", "nnwyi", "xnwyi"}},
    {"んゑ", new string[3] {"nwye", "nnwye", "xnwye"}},
    {"ん、", new string[3] {"n,", "nn,", "xn,"}},
    {"っか", new string[10] {"kka", "cca", "ltuka", "xtuka", "ltsuka", "xtsuka", "ltuca", "xtuca", "ltsuca", "xtsuca"}},
    {"っき", new string[5] {"kki", "ltuki", "xtuki", "ltsuki", "xtsuki"}},
    {"っく", new string[10] {"kku", "ccu", "ltuku", "xtuku", "ltsuku", "xtsuku", "ltucu", "xtucu", "ltsucu", "xtsucu"}},
    {"っけ", new string[5] {"kke", "ltuke", "xtuke", "ltsuke", "xtsuke"}},
    {"っこ", new string[10] {"kko", "cco", "ltuko", "xtuko", "ltsuko", "xtsuko", "ltuco", "xtuco", "ltsuco", "xtsuco"}},
    {"っさ", new string[5] {"ssa", "ltusa", "xtusa", "ltsusa", "xtsusa"}},
    {"っし", new string[15] {"ssi", "cci", "sshi", "ltusi", "xtsusi", "ltsusi", "xtsusi", "ltuci", "xtuci", "ltsuci", "xtsuci", "ltushi", "xtushi", "ltsushi", "xtsushi"}},
    {"っす", new string[5] {"ssu", "ltusu", "xtusu", "ltsusu", "xtsusu"}},
    {"っせ", new string[10] {"sse", "cce", "ltuse", "xtuse", "ltsuse", "xtsuse", "ltuce", "xtuce", "ltsuce", "xtsuce"}},
    {"っそ", new string[5] {"sso", "ltuso", "xtuso", "ltsuso", "xtsuso"}},
    {"った", new string[5] {"tta", "ltuta", "xtuta", "ltsuta", "xtsuta"}},
    {"っち", new string[10] {"tti", "cchi", "ltuti", "xtuti", "ltsuti", "xtsuti", "ltuchi", "xtuchi", "ltsuchi", "xtsuchi"}},
    {"っつ", new string[10] {"ttu", "ttsu", "ltutu", "xtutu", "ltsutu", "xtsutu", "ltutsu", "xtutsu", "ltsutsu", "xtsutsu"}},
    {"って", new string[5] {"tte", "ltute", "xtute", "ltsute", "xtsute"}},
    {"っと", new string[5] {"tto", "ltuto", "xtuto", "ltsuto", "xtsuto"}},
    {"っな", new string[4] {"ltuna", "xtuna", "ltsuna", "xtsuna"}},
    {"っに", new string[4] {"ltuni", "xtuni", "ltsuni", "xtsuni"}},
    {"っぬ", new string[4] {"ltunu", "xtunu", "ltsunu", "xtsunu"}},
    {"っね", new string[4] {"ltune", "xtune", "ltsune", "xtsune"}},
    {"っの", new string[4] {"ltuno", "xtuno", "ltsuno", "xtsuno"}},
    {"っは", new string[5] {"hha", "ltuha", "xtuha", "ltsuha", "xtsuha"}},
    {"っひ", new string[5] {"hhi", "ltuhi", "xtuhi", "ltsuhi", "xtsuhi"}},
    {"っふ", new string[10] {"ffu", "hhu", "ltufu", "xtufu", "ltsufu", "xtsufu", "ltuhu", "xtuhu", "ltsuhu", "xtsuhu"}},
    {"っへ", new string[5] {"hhe", "ltuhe", "xtuhe", "ltsuhe", "xtsuhe"}},
    {"っほ", new string[5] {"hho", "ltuho", "xtuho", "ltsuho", "xtsuho"}},
    {"っま", new string[5] {"mma", "ltuma", "xtuma", "ltsuma", "xtsuma"}},
    {"っみ", new string[5] {"mmi", "ltumi", "xtumi", "ltsumi", "xtsumi"}},
    {"っむ", new string[5] {"mmu", "ltumu", "xtumu", "ltsumu", "xtsumu"}},
    {"っめ", new string[5] {"mme", "ltume", "xtume", "ltsume", "xtsume"}},
    {"っも", new string[5] {"mmo", "ltumo", "xtumo", "ltsumo", "xtsumo"}},
    {"っや", new string[5] {"yya", "ltuya", "xtuya", "ltsuya", "xtsuya"}},
    {"っゆ", new string[5] {"yyu", "ltuyu", "xtuyu", "ltsuyu", "xtsuyu"}},
    {"っよ", new string[5] {"yyo", "ltuyo", "xtuyo", "ltsuyo", "xtsuyo"}},
    {"っら", new string[5] {"rra", "ltura", "xtura", "ltsura", "xtsura"}},
    {"っり", new string[5] {"rri", "lturi", "xturi", "ltsuri", "xtsuri"}},
    {"っる", new string[5] {"rru", "lturu", "xturu", "ltsuru", "xtsuru"}},
    {"っれ", new string[5] {"rre", "lture", "xture", "ltsure", "xtsure"}},
    {"っろ", new string[5] {"rro", "lturo", "xturo", "ltsuro", "xtsuro"}},
    {"っわ", new string[5] {"wwa", "ltuwa", "xtuwa", "ltsuwa", "xtsuwa"}},
    {"っを", new string[5] {"wwo", "ltuwo", "xtuwo", "ltsuwo", "xtsuwo"}},
    {"っが", new string[5] {"gga", "ltuga", "xtuga", "ltsuga", "xtsuga"}},
    {"っぎ", new string[5] {"ggi", "ltugi", "xtugi", "ltsugi", "xtsugi"}},
    {"っぐ", new string[5] {"ggu", "ltugu", "xtugu", "ltsugu", "xtsugu"}},
    {"っげ", new string[5] {"gge", "ltuge", "xtuge", "ltsuge", "xtsuge"}},
    {"っご", new string[5] {"ggo", "ltugo", "xtugo", "ltsugo", "xtsugo"}},
    {"っざ", new string[5] {"zza", "ltuza", "xtuza", "ltsuza", "xtsuza"}},
    {"っじ", new string[10] {"jji", "zzi", "ltuji", "xtuji", "ltsuji", "xtsuji", "ltuzi", "xtuzi", "ltsuzi", "xtsuzi"}},
    {"っず", new string[5] {"zzu", "ltuzu", "xtuzu", "ltsuzu", "xtsuzu"}},
    {"っぜ", new string[5] {"zze", "ltuze", "xtuze", "ltsuze", "xtsuze"}},
    {"っぞ", new string[5] {"zzo", "ltuzo", "xtuzo", "ltsuzo", "xtsuzo"}},
    {"っだ", new string[5] {"dda", "ltuda", "xtuda", "ltsuda", "xtsuda"}},
    {"っぢ", new string[5] {"ddi", "ltudi", "xtudi", "ltsudi", "xtsudi"}},
    {"っづ", new string[5] {"ddu", "ltudu", "xtudu", "ltsudu", "xtsudu"}},
    {"っで", new string[5] {"dde", "ltude", "xtude", "ltsude", "xtsude"}},
    {"っど", new string[5] {"ddo", "ltudo", "xtudo", "ltsudo", "xtsudo"}},
    {"っば", new string[5] {"bba", "ltuba", "xtuba", "ltsuba", "xtsuba"}},
    {"っび", new string[5] {"bbi", "ltubi", "xtubi", "ltsubi", "xtsubi"}},
    {"っぶ", new string[5] {"bbu", "ltubu", "xtubu", "ltsubu", "xtsubu"}},
    {"っべ", new string[5] {"bbe", "ltube", "xtube", "ltsube", "xtsube"}},
    {"っぼ", new string[5] {"bbo", "ltubo", "xtubo", "ltsubo", "xtsubo"}},
    {"っぱ", new string[5] {"ppa", "ltupa", "xtupa", "ltsupa", "xtsupa"}},
    {"っぴ", new string[5] {"ppi", "ltupi", "xtupi", "ltsupi", "xtsupi"}},
    {"っぷ", new string[5] {"ppu", "ltupu", "xtupu", "ltsupu", "xtsupu"}},
    {"っぺ", new string[5] {"ppe", "ltupe", "xtupe", "ltsupe", "xtsupe"}},
    {"っぽ", new string[5] {"ppo", "ltupo", "xtupo", "ltsupo", "xtsupo"}},
    {"っきゃ", new string[19] {"kkya", "kkilya", "kkixya", "ltukya", "ltukilya", "ltukixya", "ltukya", "xtukilya", "xtukixya", "xtukya", "xtukilya",
                              "ltsukixya", "ltsukya", "ltsukilya", "ltsukixya", "xtsukixya", "xtsukya", "xtsukilya", "xtsukixya"}},
    {"っきぃ", new string[25] {"kkyi", "kkili", "kkilyi", "kkixi", "kkixyi", "ltukyi", "ltukili", "ltukilyi", "ltukixi", "ltukixyi", "xtukyi", "xtukili", "xtukilyi", "xtukixi", "xtukixyi",
                            "ltsukyi", "ltsukili", "ltsukilyi", "ltsukixi", "ltsukixyi", "ltsukyi", "xtsukili", "xtsukilyi", "xtsukixi", "xtsukixyi"}},
    {"っきゅ", new string[15] {"kkyu", "kkilyu", "kkixyu", "ltukyu", "ltukilyu", "ltukixyu", "xtukyu", "xtukilyu", "xtukixyu", "ltsukyu", "ltsukilyu", "ltsukixyu", "xtsukyu", "xtsukilyu", "xtsukixyu"}},
    {"っきょ", new string[15] {"kkyo", "kkilyo", "kkixyo", "ltukyo", "xtukyo", "ltsukyo", "xtsukyo", "ltukilyo", "xtukilyo", "ltsukilyo", "xtsukilyo", "ltukixyo", "xtukixyo", "ltsukixyo", "xtsukixyo"}},
    {"っぎゃ", new string[15] {"ggya", "ggilya", "ggixya", "ltugya", "xtugya", "ltsugya", "xtsugya", "ltugilya", "xtugilya", "ltsugilya", "xtsugilya", "ltugixya", "xtugixya", "ltsugixya", "xtsugixya"}},
    {"っぎゅ", new string[15] {"ggyu", "ggilyu", "ggixyu", "ltugyu", "xtugyu", "ltsugyu", "xtsugyu", "ltugilyu", "xtugilyu", "ltsugilyu", "xtsugilyu", "ltugixyu", "xtugixyu", "ltsugixyu", "xtsugixyu"}},
    {"っぎょ", new string[15] {"ggyo", "ggilyo", "ggixyo", "ltugyo", "xtugyo", "ltsugyo", "xtsugyo", "ltugilyo", "xtugilyo", "ltsugilyo", "xtsugilyo", "ltugixyo", "xtugixyo", "ltsugixyo", "xtsugixyo"}},
    {"っしゃ", new string[40] {"ssya", "ssha", "ssilya", "ssixya", "sshilya", "sshixya", "ccilya", "ccixya", "ltusya", "xtusya", "ltsusya", "xtsusya",
                              "ltusha", "xtusha", "ltsusha", "xtsusha", "ltusilya", "xtusilya", "ltsusilya", "xtsusilya", "ltusixya", "xtusixya", "ltsusixya", "xtsusixya",
                              "ltushilya", "xtushilya", "ltsushilya", "xtsushilya", "ltushixya", "xtushixya", "ltsushixya", "xtsushixya",
                              "ltucilya", "xtucilya", "ltsucilya", "xtsucilya", "ltucixya", "xtucixya", "ltsucixya", "xtsucixya"}},
    {"っしぃ", new string[65] {"ssyi", "ssili", "ssixi", "ssilyi", "ssixyi", "sshili", "sshixi", "sshilyi", "sshixyi", "ccili", "ccixi", "ccilyi", "ccixyi",
                              "ltusyi", "xtusyi", "ltsusyi", "xtsusyi",
                              "ltusili", "xtusili", "ltsusili", "xtsusili",
                              "ltusixi", "xtusixi", "ltsusixi", "xtsusixi",
                              "ltusilyi", "xtusilyi", "ltsusilyi", "xtsusilyi",
                              "ltusixyi", "xtusixyi", "ltsusixyi", "xtsusixyi",
                              "ltushili", "xtushili", "ltsushili", "xtsushili",
                              "ltushixi", "xtushixi", "ltsushixi", "xtsushixi",
                              "ltushilyi", "xtushilyi", "ltsushilyi", "xtsushilyi",
                              "ltushixyi", "xtushixyi", "ltsushixyi", "xtsushixyi",
                              "ltucili", "xtucili", "ltsucili", "xtsucili",
                              "ltucixi", "xtucixi", "ltsucixi", "xtsucixi",
                              "ltucilyi", "xtucilyi", "ltsucilyi", "xtsucilyi",
                              "ltucixyi", "xtucixyi", "ltsucixyi", "xtsucixyi"}},
    {"っしゅ", new string[40] {"ssyu", "sshu", "ssilyu", "ssixyu", "sshilyu", "sshixyu", "ccilyu", "ccixyu", "ltusyu", "xtusyu", "ltsusyu", "xtsusyu",
                              "ltushu", "xtushu", "ltsushu", "xtsushu",
                              "ltusilyu", "xtusilyu", "ltsusilyu", "xtsusilyu",
                              "ltusixyu", "xtusixyu", "ltsusixyu", "xtsusixyu",
                              "ltushilyu", "xtushilyu", "ltsushilyu", "xtsushilyu",
                              "ltushixyu", "xtushixyu", "ltsushixyu", "xtsushixyu",
                              "ltucilyu", "xtucilyu", "ltsucilyu", "xtsucilyu",
                              "ltucixyu", "xtucixyu", "ltsucixyu", "xtsucixyu"}},
    {"っしぇ", new string[70] {"ssye", "sshe", "ssile", "ssilye", "ssixe", "ssixye", "sshile", "sshilye", "sshixe", "sshixye", "ccile", "ccilye", "ccixe", "ccixye", "ltusye", "xtusye", "ltsusye", "xtsusye",
                              "ltushe", "xtushe", "ltsushe", "xtsushe",
                              "ltusile", "xtusile", "ltsusile", "xtsusile",
                              "ltusilye", "xtusilye", "ltsusilye", "xtsusilye",
                              "ltusixe", "xtusixe", "ltsusixe", "xtsusixe",
                              "ltusixye", "xtusixye", "ltsusixye", "xtsusixye",
                              "ltushile", "xtushile", "ltsushile", "xtsushile",
                              "ltushilye", "xtushilye", "ltsushilye", "xtsushilye",
                              "ltushixe", "xtushixe", "ltsushixe", "xtsushixe",
                              "ltushixye", "xtushixye", "ltsushixye", "xtsushixye",
                              "ltucile", "xtucile", "ltsucile", "xtsucile",
                              "ltucilye", "xtucilye", "ltsucilye", "xtsucilye",
                              "ltucixe", "xtucixe", "ltsucixe", "xtsucixe",
                              "ltucixye", "xtucixye", "ltsucixye", "xtsucixye"}},
    {"っしょ", new string[40] {"ssyo", "ssho", "ssilyo", "ssixyo", "sshilyo", "sshixyo", "ccilyo", "ccixyo", "ltusyo", "xtusyo", "ltsusyo", "xtsusyo",
                              "ltusho", "xtusho", "ltsusho", "xtsusho",
                              "ltusilyo", "xtusilyo", "ltsusilyo", "xtsusilyo",
                              "ltusixyo", "xtusixyo", "ltsusixyo", "xtsusixyo",
                              "ltushilyo", "xtushilyo", "ltsushilyo", "xtsushilyo",
                              "ltushixyo", "xtushixyo", "ltsushixyo", "xtsushixyo",
                              "ltucilyo", "xtucilyo", "ltsucilyo", "xtsucilyo",
                              "ltucixyo", "xtucixyo", "ltsucixyo", "xtsucixyo"}},
    {"っじゃ", new string[35] {"jja", "jjya", "zzya", "jjilya", "jjixya", "zzilya", "zzixya", "ltuja", "xtuja", "ltsuja", "xtsuja",
                              "ltujya", "xtujya", "ltsujya", "xtsujya",
                              "ltuzya", "xtuzya", "ltsuzya", "xtsuzya",
                              "ltujilya", "xtujilya", "ltsujilya", "xtsujilya",
                              "ltujixya", "xtujixya", "ltsujixya", "xtsujixya",
                              "ltuzilya", "xtuzilya", "ltsuzilya", "xtsuzilya",
                              "ltuzixya", "xtuzixya", "ltsuzixya", "xtsuzixya"}},
    {"っじぃ", new string[50] {"jjyi", "zzyi", "jjili", "jjixi", "jjilyi", "jjixyi", "zzili", "zzixi", "zzilyi", "zzixyi", "ltujyi", "xtujyi", "ltsujyi", "xtsujyi",
                              "ltuzyi", "xtuzyi", "ltsuzyi", "xtsuzyi",
                              "ltujili", "xtujili", "ltsujili", "xtsujili",
                              "ltujixi", "xtujixi", "ltsujixi", "xtsujixi",
                              "ltujilyi", "xtujilyi", "ltsujilyi", "xtsujilyi",
                              "ltujixyi", "xtujixyi", "ltsujixyi", "xtsujixyi",
                              "ltuzili", "xtuzili", "ltsuzili", "xtsuzili",
                              "ltuzixi", "xtuzixi", "ltsuzixi", "xtsuzixi",
                              "ltuzilyi", "xtuzilyi", "ltsuzilyi", "xtsuzilyi",
                              "ltuzixyi", "xtuzixyi", "ltsuzixyi", "xtsuzixyi"}},
    {"っじゅ", new string[35] {"jju", "jjyu", "zzyu", "jjilyu", "jjixyu", "zzilyu", "zzixyu", "ltuju", "xtuju", "ltsuju", "xtsuju",
                              "ltujyu", "xtujyu", "ltsujyu", "xtsujyu",
                              "ltuzyu", "xtuzyu", "ltsuzyu", "xtsuzyu",
                              "ltujilyu", "xtujilyu", "ltsujilyu", "xtsujilyu",
                              "ltujixyu", "xtujixyu", "ltsujixyu", "xtsujixyu",
                              "ltuzilyu", "xtuzilyu", "ltsuzilyu", "xtsuzilyu",
                              "ltuzixyu", "xtuzixyu", "ltsuzixyu", "xtsuzixyu"}},
    {"っじぇ", new string[55] {"jje", "jjye", "zzye", "jjile", "jjixe", "jjilye", "jjixye", "zzile", "zzixe", "zzilye", "zzixye", "ltuje", "xtuje", "ltsuje", "xtsuje",
                              "ltujye", "xtujye", "ltsujye", "xtsujye",
                              "ltuzye", "xtuzye", "ltsuzye", "xtsuzye",
                              "ltujile", "xtujile", "ltsujile", "xtsujile",
                              "ltujixe", "xtujixe", "ltsujixe", "xtsujixe",
                              "ltujilye", "xtujilye", "ltsujilye", "xtsujilye",
                              "ltujixye", "xtujixye", "ltsujixye", "xtsujixye",
                              "ltuzile", "xtuzile", "ltsuzile", "xtsuzile",
                              "ltuzixe", "xtuzixe", "ltsuzixe", "xtsuzixe",
                              "ltuzilye", "xtuzilye", "ltsuzilye", "xtsuzilye",
                              "ltuzixye", "xtuzixye", "ltsuzixye", "xtsuzixye"}},
    {"っじょ", new string[35] {"jjo", "jjyo", "zzyo", "jjilyo", "jjixyo", "zzilyo", "zzixyo", "ltujo", "xtujo", "ltsujo", "xtsujo",
                              "ltujyo", "xtujyo", "ltsujyo", "xtsujyo",
                              "ltuzyo", "xtuzyo", "ltsuzyo", "xtsuzyo",
                              "ltujilyo", "xtujilyo", "ltsujilyo", "xtsujilyo",
                              "ltujixyo", "xtujixyo", "ltsujixyo", "xtsujixyo",
                              "ltuzilyo", "xtuzilyo", "ltsuzilyo", "xtsuzilyo",
                              "ltuzixyo", "xtuzixyo", "ltsuzixyo", "xtsuzixyo"}},
    {"っちゃ", new string[35] {"ttya", "ccha", "ccya", "ttilya", "ttixya", "cchilya", "cchixya", "ltutya", "xtutya", "ltsutya", "xtsutya",
                              "ltucha", "xtucha", "ltsucha", "xtsucha",
                              "ltucya", "xtucya", "ltsucya", "xtsucya",
                              "ltutilya", "xtutilya", "ltsutilya", "xtsutilya",
                              "ltutixya", "xtutixya", "ltsutixya", "xtsutixya",
                              "ltuchilya", "xtuchilya", "ltsuchilya", "xtsuchilya",
                              "ltuchixya", "xtuchixya", "ltsuchixya", "xtsuchixya"}},
    {"っちぃ", new string[50] {"ttyi", "ccyi", "ttili", "ttixi", "ttilyi", "ttixyi", "cchili", "cchixi", "cchilyi", "cchixyi", "ltutyi", "xtutyi", "ltsutyi", "xtsutyi",
                              "ltucyi", "xtucyi", "ltsucyi", "xtsucyi",
                              "ltutili", "xtutili", "ltsutili", "xtsutili",
                              "ltutixi", "xtutixi", "ltsutixi", "xtsutixi",
                              "ltutilyi", "xtutilyi", "ltsutilyi", "xtsutilyi",
                              "ltutixyi", "xtutixyi", "ltsutixyi", "xtsutixyi",
                              "ltuchili", "xtuchili", "ltsuchili", "xtsuchili",
                              "ltuchixi", "xtuchixi", "ltsuchixi", "xtsuchixi",
                              "ltuchilyi", "xtuchilyi", "ltsuchilyi", "xtsuchilyi",
                              "ltuchixyi", "xtuchixyi", "ltsuchixyi", "xtsuchixyi"}},
    {"っちゅ", new string[35] {"ttyu", "cchu", "ccyu", "ttilyu", "ttixyu", "cchilyu", "cchixyu", "ltutyu", "xtutyu", "ltsutyu", "xtsutyu",
                              "ltuchu", "xtuchu", "ltsuchu", "xtsuchu",
                              "ltucyu", "xtucyu", "ltsucyu", "xtsucyu",
                              "ltutilyu", "xtutilyu", "ltsutilyu", "xtsutilyu",
                              "ltutixyu", "xtutixyu", "ltsutixyu", "xtsutixyu",
                              "ltuchilyu", "xtuchilyu", "ltsuchilyu", "xtsuchilyu",
                              "ltuchixyu", "xtuchixyu", "ltsuchixyu", "xtsuchixyu"}},
    {"っちぇ", new string[55] {"ttye", "cche", "ccye", "ttile", "ttixe", "ttilye", "ttixye", "cchile", "cchixe", "cchilye", "cchixye", "ltutye", "xtutye", "ltsutye", "xtsutye",
                              "ltuche", "xtuche", "ltsuche", "xtsuche",
                              "ltucye", "xtucye", "ltsucye", "xtsucye",
                              "ltutile", "xtutile", "ltsutile", "xtsutile",
                              "ltutixe", "xtutixe", "ltsutixe", "xtsutixe",
                              "ltutilye", "xtutilye", "ltsutilye", "xtsutilye",
                              "ltutixye", "xtutixye", "ltsutixye", "xtsutixye",
                              "ltuchile", "xtuchile", "ltsuchile", "xtsuchile",
                              "ltuchixe", "xtuchixe", "ltsuchixe", "xtsuchixe",
                              "ltuchilye", "xtuchilye", "ltsuchilye", "xtsuchilye",
                              "ltuchixye", "xtuchixye", "ltsuchixye", "xtsuchixye"}},
    {"っちょ", new string[35] {"ttyo", "ccho", "ccyo", "ttilyo", "ttixyo", "cchilyo", "cchixyo", "ltutyo", "xtutyo", "ltsutyo", "xtsutyo",
                              "ltucho", "xtucho", "ltsucho", "xtsucho",
                              "ltucyo", "xtucyo", "ltsucyo", "xtsucyo",
                              "ltutilyo", "xtutilyo", "ltsutilyo", "xtsutilyo",
                              "ltutixyo", "xtutixyo", "ltsutixyo", "xtsutixyo",
                              "ltuchilyo", "xtuchilyo", "ltsuchilyo", "xtsuchilyo",
                              "ltuchixyo", "xtuchixyo", "ltsuchixyo", "xtsuchixyo"}},
    {"っぢゃ", new string[15] {"ddya", "ddilya", "ddixya", "ltudya", "xtudya", "ltsudya", "xtsudya",
                              "ltudilya", "xtudilya", "ltsudilya", "xtsudilya",
                              "ltudixya", "xtudixya", "ltsudixya", "xtsudixya"}},
    {"っぢゅ", new string[15] {"ddyu", "ddilyu", "ddixyu", "ltudyu", "xtudyu", "ltsudyu", "xtsudyu",
                              "ltudilyu", "xtudilyu", "ltsudilyu", "xtsudilyu",
                              "ltudixyu", "xtudixyu", "ltsudixyu", "xtsudixyu"}},
    {"っぢぇ", new string[25] {"ddye", "ddile", "ddixe", "ddilye", "ddixye", "ltudye", "xtudye", "ltsudye", "xtsudye",
                              "ltudile", "xtudile", "ltsudile", "xtsudile",
                              "ltudixe", "xtudixe", "ltsudixe", "xtsudixe",
                              "ltudilye", "xtudilye", "ltsudilye", "xtsudilye",
                              "ltudixye", "xtudixye", "ltsudixye", "xtsudixye"}},
    {"っぢょ", new string[15] {"ddyo", "ddilyo", "ddixyo", "ltudyo", "xtudyo", "ltsudyo", "xtsudyo",
                              "ltudilyo", "xtudilyo", "ltsudilyo", "xtsudilyo",
                              "ltudixyo", "xtudixyo", "ltsudixyo", "xtsudixyo"}},
    {"っつぁ", new string[25] {"ttsa", "ttula", "ttuxa", "ttsula", "ttsuxa", "ltutsa", "xtutsa", "ltsutsa", "xtsutsa",
                              "ltutula", "xtutula", "ltsutula", "xtsutula",
                              "ltutuxa", "xtutuxa", "ltsutuxa", "xtsutuxa",
                              "ltutsula", "xtutsula", "ltsutsula", "xtsutsula",
                              "ltutsuxa", "xtutsuxa", "ltsutsuxa", "xtsutsuxa"}},
    {"っつぃ", new string[45] {"ttsi", "ttuli", "ttuxi", "ttulyi", "ttuxyi", "ttsuli", "ttsuxi", "ttsulyi", "ttsuxyi", "ltutsi", "xtutsi", "ltsutsi", "xtsutsi",
                              "ltutuli", "xtutuli", "ltsutuli", "xtsutuli",
                              "ltutuxi", "xtutuxi", "ltsutuxi", "xtsutuxi",
                              "ltutulyi", "xtutulyi", "ltsutulyi", "xtsutulyi",
                              "ltutuxyi", "xtutuxyi", "ltsutuxyi", "xtsutuxyi",
                              "ltutsuli", "xtutsuli", "ltsutsuli", "xtsutsuli",
                              "ltutsuxi", "xtutsuxi", "ltsutsuxi", "xtsutsuxi",
                              "ltutsulyi", "xtutsulyi", "ltsutsulyi", "xtsutsulyi",
                              "ltutsuxyi", "xtutsuxyi", "ltsutsuxyi", "xtsutsuxyi"}},
    {"っつぇ", new string[45] {"ttse", "ttule", "ttuxe", "ttulye", "ttuxye", "ttsule", "ttsuxe", "ttsulye", "ttsuxye", "ltutse", "xtutse", "ltsutse", "xtsutse",
                              "ltutule", "xtutule", "ltsutule", "xtsutule",
                              "ltutuxe", "xtutuxe", "ltsutuxe", "xtsutuxe",
                              "ltutulye", "xtutulye", "ltsutulye", "xtsutulye",
                              "ltutuxye", "xtutuxye", "ltsutuxye", "xtsutuxye",
                              "ltutsule", "xtutsule", "ltsutsule", "xtsutsule",
                              "ltutsuxe", "xtutsuxe", "ltsutsuxe", "xtsutsuxe",
                              "ltutsulye", "xtutsulye", "ltsutsulye", "xtsutsulye",
                              "ltutsuxye", "xtutsuxye", "ltsutsuxye", "xtsutsuxye"}},
    {"っつぉ", new string[25] {"ttso", "ttulo", "ttuxo", "ttsulo", "ttsuxo", "ltutso", "xtutso", "ltsutso", "xtsutso",
                              "ltutulo", "xtutulo", "ltsutulo", "xtsutulo",
                              "ltutuxo", "xtutuxo", "ltsutuxo", "xtsutuxo",
                              "ltutsulo", "xtutsulo", "ltsutsulo", "xtsutsulo",
                              "ltutsuxo", "xtutsuxo", "ltsutsuxo", "xtsutsuxo"}},
    {"ってゃ", new string[15] {"ttha", "ttelya", "ttexya", "ltutha", "xtutha", "ltsutha", "xtsutha",
                              "ltutelya", "xtutelya", "ltsutelya", "xtsutelya",
                              "ltutexya", "xtutexya", "ltsutexya", "xtsutexya"}},
    {"ってぃ", new string[25] {"tthi", "tteli", "ttexi", "ttelyi", "ttexyi", "ltuthi", "xtuthi", "ltsuthi", "xtsuthi",
                              "ltuteli", "xtuteli", "ltsuteli", "xtsuteli",
                              "ltutexi", "xtutexi", "ltsutexi", "xtsutexi",
                              "ltutelyi", "xtutelyi", "ltsutelyi", "xtsutelyi",
                              "ltutexyi", "xtutexyi", "ltsutexyi", "xtsutexyi"}},
    {"ってゅ", new string[15] {"tthu", "ttelyu", "ttexyu", "ltuthu", "xtuthu", "ltsuthu", "xtsuthu",
                              "ltutelyu", "xtutelyu", "ltsutelyu", "xtsutelyu",
                              "ltutexyu", "xtutexyu", "ltsutexyu", "xtsutexyu"}},
    {"ってぇ", new string[25] {"tthe", "ttele", "ttexe", "ttelye", "ttexye", "ltuthe", "xtuthe", "ltsuthe", "xtsuthe",
                              "ltutele", "xtutele", "ltsutele", "xtsutele",
                              "ltutexe", "xtutexe", "ltsutexe", "xtsutexe",
                              "ltutelye", "xtutelye", "ltsutelye", "xtsutelye",
                              "ltutexye", "xtutexye", "ltsutexye", "xtsutexye"}},
    {"ってょ", new string[15] {"ttho", "ttelyo", "ttexyo", "ltutho", "xtutho", "ltsutho", "xtsutho",
                              "ltutelyo", "xtutelyo", "ltsutelyo", "xtsutelyo",
                              "ltutexyo", "xtutexyo", "ltsutexyo", "xtsutexyo"}},
    {"っでゃ", new string[15] {"ddha", "ddelya", "ddexya", "ltudha", "xtudha", "ltsudha", "xtsudha",
                              "ltudelya", "xtudelya", "ltsudelya", "xtsudelya",
                              "ltudexya", "xtudexya", "ltsudexya", "xtsudexya"}},
    {"っでぃ", new string[25] {"ddhi", "ddeli", "ddexi", "ddelyi", "ddexyi", "ltudhi", "xtudhi", "ltsudhi", "xtsudhi",
                              "ltudeli", "xtudeli", "ltsudeli", "xtsudeli",
                              "ltudexi", "xtudexi", "ltsudexi", "xtsudexi",
                              "ltudelyi", "xtudelyi", "ltsudelyi", "xtsudelyi",
                              "ltudexyi", "xtudexyi", "ltsudexyi", "xtsudexyi"}},
    {"っでゅ", new string[15] {"ddhu", "ddelyu", "ddexyu", "ltudhu", "xtudhu", "ltsudhu", "xtsudhu",
                              "ltudelyu", "xtudelyu", "ltsudelyu", "xtsudelyu",
                              "ltudexyu", "xtudexyu", "ltsudexyu", "xtsudexyu"}},
    {"っでぇ", new string[25] {"ddhe", "ddele", "ddexe", "ddelye", "ddexye", "ltudhe", "xtudhe", "ltsudhe", "xtsudhe",
                              "ltudele", "xtudele", "ltsudele", "xtsudele",
                              "ltudexe", "xtudexe", "ltsudexe", "xtsudexe",
                              "ltudelye", "xtudelye", "ltsudelye", "xtsudelye",
                              "ltudexye", "xtudexye", "ltsudexye", "xtsudexye"}},
    {"っでょ", new string[15] {"ddho", "ddelyo", "ddexyo", "ltudho", "xtudho", "ltsudho", "xtsudho",
                              "ltudelyo", "xtudelyo", "ltsudelyo", "xtsudelyo",
                              "ltudexyo", "xtudexyo", "ltsudexyo", "xtsudexyo"}},
    {"っぴゃ", new string[15] {"ppya", "ppilya", "ppixya", "ltupya", "xtupya", "ltsupya", "xtsupya",
                              "ltupilya", "xtupilya", "ltsupilya", "xtsupilya",
                              "ltupixya", "xtupixya", "ltsupixya", "xtsupixya"}},
    {"っぴぃ", new string[25] {"ppyi", "ppili", "ppixi", "ppilyi", "ppixyi", "ltupyi", "xtupyi", "ltsupyi", "xtsupyi",
                              "ltupili", "xtupili", "ltsupili", "xtsupili",
                              "ltupixi", "xtupixi", "ltsupixi", "xtsupixi",
                              "ltupilyi", "xtupilyi", "ltsupilyi", "xtsupilyi",
                              "ltupixyi", "xtupixyi", "ltsupixyi", "xtsupixyi"}},
    {"っぴゅ", new string[15] {"ppyu", "ppilyu", "ppixyu", "ltupyu", "xtupyu", "ltsupyu", "xtsupyu",
                              "ltupilyu", "xtupilyu", "ltsupilyu", "xtsupilyu",
                              "ltupixyu", "xtupixyu", "ltsupixyu", "xtsupixyu"}},
    {"っぴぇ", new string[25] {"ppye", "ppile", "ppixe", "ppilye", "ppixye", "ltupye", "xtupye", "ltsupye", "xtsupye",
                              "ltupile", "xtupile", "ltsupile", "xtsupile",
                              "ltupixe", "xtupixe", "ltsupixe", "xtsupixe",
                              "ltupilye", "xtupilye", "ltsupilye", "xtsupilye",
                              "ltupixye", "xtupixye", "ltsupixye", "xtsupixye"}},
    {"っぴょ", new string[15] {"ppyo", "ppilyo", "ppixyo", "ltupyo", "xtupyo", "ltsupyo", "xtsupyo",
                              "ltupilyo", "xtupilyo", "ltsupilyo", "xtsupilyo",
                              "ltupixyo", "xtupixyo", "ltsupixyo", "xtsupixyo"}},
    {"っひゃ", new string[15] {"hhya", "hhilya", "hhixya", "ltuhya", "xtuhya", "ltsuhya", "xtsuhya",
                              "ltuhilya", "xtuhilya", "ltsuhilya", "xtsuhilya",
                              "ltuhixya", "xtuhixya", "ltsuhixya", "xtsuhixya"}},
    {"っひぃ", new string[25] {"hhyi", "hhili", "hhixi", "hhilyi", "hhixyi", "ltuhyi", "xtuhyi", "ltsuhyi", "xtsuhyi",
                              "ltuhili", "xtuhili", "ltsuhili", "xtsuhili",
                              "ltuhixi", "xtuhixi", "ltsuhixi", "xtsuhixi",
                              "ltuhilyi", "xtuhilyi", "ltsuhilyi", "xtsuhilyi",
                              "ltuhixyi", "xtuhixyi", "ltsuhixyi", "xtsuhixyi"}},
    {"っひゅ", new string[15] {"hhyu", "hhilyu", "hhixyu", "ltuhyu", "xtuhyu", "ltsuhyu", "xtsuhyu",
                              "ltuhilyu", "xtuhilyu", "ltsuhilyu", "xtsuhilyu",
                              "ltuhixyu", "xtuhixyu", "ltsuhixyu", "xtsuhixyu"}},
    {"っひぇ", new string[25] {"hhye", "hhile", "hhixe", "hhilye", "hhixye", "ltuhye", "xtuhye", "ltsuhye", "xtsuhye",
                              "ltuhile", "xtuhile", "ltsuhile", "xtsuhile",
                              "ltuhixe", "xtuhixe", "ltsuhixe", "xtsuhixe",
                              "ltuhilye", "xtuhilye", "ltsuhilye", "xtsuhilye",
                              "ltuhixye", "xtuhixye", "ltsuhixye", "xtsuhixye"}},
    {"っひょ", new string[15] {"hhyo", "hhilyo", "hhixyo", "ltuhyo", "xtuhyo", "ltsuhyo", "xtsuhyo",
                              "ltuhilyo", "xtuhilyo", "ltsuhilyo", "xtsuhilyo",
                              "ltuhixyo", "xtuhixyo", "ltsuhixyo", "xtsuhixyo"}},
    {"っびゃ", new string[15] {"bbya", "bbilya", "bbixya", "ltubya", "xtubya", "ltsubya", "xtsubya",
                              "ltubilya", "xtubilya", "ltsubilya", "xtsubilya",
                              "ltubixya", "xtubixya", "ltsubixya", "xtsubixya"}},
    {"っびぃ", new string[25] {"bbyi", "bbili", "bbixi", "bbilyi", "bbixyi", "ltubyi", "xtubyi", "ltsubyi", "xtsubyi",
                            "ltubili", "xtubili", "ltsubili", "xtsubili",
                            "ltubixi", "xtubixi", "ltsubixi", "xtsubixi",
                            "ltubilyi", "xtubilyi", "ltsubilyi", "xtsubilyi",
                            "ltubixyi", "xtubixyi", "ltsubixyi", "xtsubixyi"}},
    {"っびゅ", new string[15] {"bbyu", "bbilyu", "bbixyu", "ltubyu", "xtubyu", "ltsubyu", "xtsubyu",
                              "ltubilyu", "xtubilyu", "ltsubilyu", "xtsubilyu",
                              "ltubixyu", "xtubixyu", "ltsubixyu", "xtsubixyu"}},
    {"っびぇ", new string[25] {"bbye", "bbile", "bbixe", "bbilye", "bbixye", "ltubye", "xtubye", "ltsubye", "xtsubye",
                              "ltubile", "xtubile", "ltsubile", "xtsubile",
                              "ltubixe", "xtubixe", "ltsubixe", "xtsubixe",
                              "ltubilye", "xtubilye", "ltsubilye", "xtsubilye",
                              "ltubixye", "xtubixye", "ltsubixye", "xtsubixye"}},
    {"っびょ", new string[15] {"bbyo", "bbilyo", "bbixyo", "ltubyo", "xtubyo", "ltsubyo", "xtsubyo",
                              "ltubilyo", "xtubilyo", "ltsubilyo", "xtsubilyo",
                              "ltubixyo", "xtubixyo", "ltsubixyo", "xtsubixyo"}},
    {"っふぁ", new string[25] {"ffa", "ffula", "ffuxa", "hhula", "hhuxa", "ltufa", "xtufa", "ltsufa", "xtsufa",
                            "ltufula", "xtufula", "ltsufula", "xtsufula",
                            "ltufuxa", "xtufuxa", "ltsufuxa", "xtsufuxa",
                            "ltuhula", "xtuhula", "ltsuhula", "xtsuhula",
                            "ltuhuxa", "xtuhuxa", "ltsuhuxa", "xtsuhuxa"}},
    {"っふぃ", new string[50] {"ffi", "ffyi", "ffuli", "ffuxi", "ffulyi", "ffuxyi", "hhuli", "hhuxi", "hhulyi", "hhuxyi", "ltufi", "xtufi", "ltsufi", "xtsufi",
                              "ltufyi", "xtufyi", "ltsufyi", "xtsufyi",
                              "ltufuli", "xtufuli", "ltsufuli", "xtsufuli",
                              "ltufuxi", "xtufuxi", "ltsufuxi", "xtsufuxi",
                              "ltufulyi", "xtufulyi", "ltsufulyi", "xtsufulyi",
                              "ltufuxyi", "xtufuxyi", "ltsufuxyi", "xtsufuxyi",
                              "ltuhuli", "xtuhuli", "ltsuhuli", "xtsuhuli",
                              "ltuhuxi", "xtuhuxi", "ltsuhuxi", "xtsuhuxi",
                              "ltuhulyi", "xtuhulyi", "ltsuhulyi", "xtsuhulyi",
                              "ltuhuxyi", "xtuhuxyi", "ltsuhuxyi", "xtsuhuxyi"}},
    {"っふぇ", new string[50] {"ffe", "ffye", "ffule", "ffuxe", "ffulye", "ffuxye", "hhule", "hhuxe", "hhulye", "hhuxye", "ltufe", "xtufe", "ltsufe", "xtsufe",
                              "ltufye", "xtufye", "ltsufye", "xtsufye",
                              "ltufule", "xtufule", "ltsufule", "xtsufule",
                              "ltufuxe", "xtufuxe", "ltsufuxe", "xtsufuxe",
                              "ltufulye", "xtufulye", "ltsufulye", "xtsufulye",
                              "ltufuxye", "xtufuxye", "ltsufuxye", "xtsufuxye",
                              "ltuhule", "xtuhule", "ltsuhule", "xtsuhule",
                              "ltuhuxe", "xtuhuxe", "ltsuhuxe", "xtsuhuxe",
                              "ltuhulye", "xtuhulye", "ltsuhulye", "xtsuhulye",
                              "ltuhuxye", "xtuhuxye", "ltsuhuxye", "xtsuhuxye"}},
    {"っふぉ", new string[25] {"ffo", "ffulo", "ffuxo", "hhulo", "hhuxo", "ltufo", "xtufo", "ltsufo", "xtsufo",
                              "ltufulo", "xtufulo", "ltsufulo", "xtsufulo",
                              "ltufuxo", "xtufuxo", "ltsufuxo", "xtsufuxo",
                              "ltuhulo", "xtuhulo", "ltsuhulo", "xtsuhulo",
                              "ltuhuxo", "xtuhuxo", "ltsuhuxo", "xtsuhuxo"}},
    {"っみゃ", new string[15] {"mmya", "mmilya", "mmixya", "ltumya", "xtumya", "ltsumya", "xtsumya",
                            "ltumilya", "xtumilya", "ltsumilya", "xtsumilya",
                            "ltumixya", "xtumixya", "ltsumixya", "xtsumixya"}},
    {"っみぃ", new string[25] {"mmyi", "mmili", "mmixi", "mmilyi", "mmixyi", "ltumyi", "xtumyi", "ltsumyi", "xtsumyi",
                            "ltumili", "xtumili", "ltsumili", "xtsumili",
                            "ltumixi", "xtumixi", "ltsumixi", "xtsumixi",
                            "ltumilyi", "xtumilyi", "ltsumilyi", "xtsumilyi",
                            "ltumixyi", "xtumixyi", "ltsumixyi", "xtsumixyi"}},
    {"っみゅ", new string[15] {"mmyu", "mmilyu", "mmixyu", "ltumyu", "xtumyu", "ltsumyu", "xtsumyu",
                            "ltumilyu", "xtumilyu", "ltsumilyu", "xtsumilyu",
                            "ltumixyu", "xtumixyu", "ltsumixyu", "xtsumixyu"}},
    {"っみぇ", new string[25] {"mmye", "mmile", "mmixe", "mmilye", "mmixye", "ltumye", "xtumye", "ltsumye", "xtsumye",
                            "ltumile", "xtumile", "ltsumile", "xtsumile",
                            "ltumixe", "xtumixe", "ltsumixe", "xtsumixe",
                            "ltumilye", "xtumilye", "ltsumilye", "xtsumilye",
                            "ltumixye", "xtumixye", "ltsumixye", "xtsumixye"}},
    {"っみょ", new string[15] {"mmyo", "mmilyo", "mmixyo", "ltumyo", "xtumyo", "ltsumyo", "xtsumyo",
                              "ltumilyo", "xtumilyo", "ltsumilyo", "xtsumilyo",
                              "ltumixyo", "xtumixyo", "ltsumixyo", "xtsumixyo"}},
    {"っりゃ", new string[15] {"rrya", "rrilya", "rrixya", "lturya", "xturya", "ltsurya", "xtsurya",
                            "lturilya", "xturilya", "ltsurilya", "xtsurilya",
                            "lturixya", "xturixya", "ltsurixya", "xtsurixya"}},
    {"っりぃ", new string[25] {"rryi", "rrili", "rrixi", "rrilyi", "rrixyi", "lturyi", "xturyi", "ltsuryi", "xtsuryi",
                              "lturili", "xturili", "ltsurili", "xtsurili",
                              "lturixi", "xturixi", "ltsurixi", "xtsurixi",
                              "lturilyi", "xturilyi", "ltsurilyi", "xtsurilyi",
                              "lturixyi", "xturixyi", "ltsurixyi", "xtsurixyi"}},
    {"っりゅ", new string[15] {"rryu", "rrilyu", "rrixyu", "lturyu", "xturyu", "ltsuryu", "xtsuryu",
                              "lturilyu", "xturilyu", "ltsurilyu", "xtsurilyu",
                              "lturixyu", "xturixyu", "ltsurixyu", "xtsurixyu"}},
    {"っりぇ", new string[25] {"rrye", "rrile", "rrixe", "rrilye", "rrixye", "lturye", "xturye", "ltsurye", "xtsurye",
                              "lturile", "xturile", "ltsurile", "xtsurile",
                              "lturixe", "xturixe", "ltsurixe", "xtsurixe",
                              "lturilye", "xturilye", "ltsurilye", "xtsurilye",
                              "lturixye", "xturixye", "ltsurixye", "xtsurixye"}},
    {"っりょ", new string[15] {"rryo", "rrilyo", "rrixyo", "lturyo", "xturyo", "ltsuryo", "xtsuryo",
                            "lturilyo", "xturilyo", "ltsurilyo", "xtsurilyo",
                            "lturixyo", "xturixyo", "ltsurixyo", "xtsurixyo"}},
    {"ー", new string[1] {"-"}},
    {"、", new string[1] {","}},
    {"。", new string[1] {"."}},
    {"0", new string[1] {"0"}},
    {"1", new string[1] {"1"}},
    {"2", new string[1] {"2"}},
    {"3", new string[1] {"3"}},
    {"4", new string[1] {"4"}},
    {"5", new string[1] {"5"}},
    {"6", new string[1] {"6"}},
    {"7", new string[1] {"7"}},
    {"8", new string[1] {"8"}},
    {"9", new string[1] {"9"}},
    {"a", new string[1] {"a"}},
    {"b", new string[1] {"b"}},
    {"c", new string[1] {"c"}},
    {"d", new string[1] {"d"}},
    {"e", new string[1] {"e"}},
    {"f", new string[1] {"f"}},
    {"g", new string[1] {"g"}},
    {"h", new string[1] {"h"}},
    {"i", new string[1] {"i"}},
    {"j", new string[1] {"j"}},
    {"k", new string[1] {"k"}},
    {"l", new string[1] {"l"}},
    {"m", new string[1] {"m"}},
    {"n", new string[1] {"n"}},
    {"o", new string[1] {"o"}},
    {"p", new string[1] {"p"}},
    {"q", new string[1] {"q"}},
    {"r", new string[1] {"r"}},
    {"s", new string[1] {"s"}},
    {"t", new string[1] {"t"}},
    {"u", new string[1] {"u"}},
    {"v", new string[1] {"v"}},
    {"w", new string[1] {"w"}},
    {"x", new string[1] {"x"}},
    {"y", new string[1] {"y"}},
    {"z", new string[1] {"z"}},
    {"A", new string[1] {"A"}},
    {"B", new string[1] {"B"}},
    {"C", new string[1] {"C"}},
    {"D", new string[1] {"D"}},
    {"E", new string[1] {"E"}},
    {"F", new string[1] {"F"}},
    {"G", new string[1] {"G"}},
    {"H", new string[1] {"H"}},
    {"I", new string[1] {"I"}},
    {"J", new string[1] {"J"}},
    {"K", new string[1] {"K"}},
    {"L", new string[1] {"L"}},
    {"M", new string[1] {"M"}},
    {"N", new string[1] {"N"}},
    {"O", new string[1] {"O"}},
    {"P", new string[1] {"P"}},
    {"Q", new string[1] {"Q"}},
    {"R", new string[1] {"R"}},
    {"S", new string[1] {"S"}},
    {"T", new string[1] {"T"}},
    {"U", new string[1] {"U"}},
    {"V", new string[1] {"V"}},
    {"W", new string[1] {"W"}},
    {"X", new string[1] {"X"}},
    {"Y", new string[1] {"Y"}},
    {"Z", new string[1] {"Z"}},
    {"んa", new string[2] {"nna", "xna"}},
    {"んb", new string[2] {"nnb", "xnb"}},
    {"んc", new string[2] {"nnc", "xnc"}},
    {"んd", new string[2] {"nnd", "xnd"}},
    {"んe", new string[2] {"nne", "xne"}},
    {"んf", new string[2] {"nnf", "xnf"}},
    {"んg", new string[2] {"nng", "xng"}},
    {"んh", new string[2] {"nnh", "xnh"}},
    {"んi", new string[2] {"nni", "xni"}},
    {"んj", new string[2] {"nnj", "xnj"}},
    {"んk", new string[2] {"nnk", "xnk"}},
    {"んl", new string[2] {"nnl", "xnl"}},
    {"んm", new string[2] {"nnm", "xnm"}},
    {"んn", new string[2] {"nnn", "xnn"}},
    {"んo", new string[2] {"nno", "xno"}},
    {"んp", new string[2] {"nnp", "xnp"}},
    {"んq", new string[2] {"nnq", "xnq"}},
    {"んr", new string[2] {"nnr", "xnr"}},
    {"んs", new string[2] {"nns", "xns"}},
    {"んt", new string[2] {"nnt", "xnt"}},
    {"んu", new string[2] {"nnu", "xnu"}},
    {"んv", new string[2] {"nnv", "xnv"}},
    {"んw", new string[2] {"nnw", "xnw"}},
    {"んx", new string[2] {"nnx", "xnx"}},
    {"んy", new string[2] {"nny", "xny"}},
    {"んz", new string[2] {"nnz", "xnz"}},
    {"んA", new string[2] {"nnA", "xnA"}},
    {"んB", new string[2] {"nnB", "xnB"}},
    {"んC", new string[2] {"nnC", "xnC"}},
    {"んD", new string[2] {"nnD", "xnD"}},
    {"んE", new string[2] {"nnE", "xnE"}},
    {"んF", new string[2] {"nnF", "xnF"}},
    {"んG", new string[2] {"nnG", "xnG"}},
    {"んH", new string[2] {"nnH", "xnH"}},
    {"んI", new string[2] {"nnI", "xnI"}},
    {"んJ", new string[2] {"nnJ", "xnJ"}},
    {"んK", new string[2] {"nnK", "xnK"}},
    {"んL", new string[2] {"nnL", "xnL"}},
    {"んM", new string[2] {"nnM", "xnM"}},
    {"んN", new string[2] {"nnN", "xnN"}},
    {"んO", new string[2] {"nnO", "xnO"}},
    {"んP", new string[2] {"nnP", "xnP"}},
    {"んQ", new string[2] {"nnQ", "xnQ"}},
    {"んR", new string[2] {"nnR", "xnR"}},
    {"んS", new string[2] {"nnS", "xnS"}},
    {"んT", new string[2] {"nnT", "xnT"}},
    {"んU", new string[2] {"nnU", "xnU"}},
    {"んV", new string[2] {"nnV", "xnV"}},
    {"んW", new string[2] {"nnW", "xnW"}},
    {"んX", new string[2] {"nnX", "xnX"}},
    {"んY", new string[2] {"nnY", "xnY"}},
    {"んZ", new string[2] {"nnZ", "xnZ"}},
    {" ", new string[1] {" "}},
    {"-", new string[1] {"-"}},
    {",", new string[1] {","}},
    {".", new string[1] {"."}},
    {";", new string[1] {";"}},
    {":", new string[1] {":"}},
    {"[", new string[1] {"["}},
    {"]", new string[1] {"]"}},
    {"@", new string[1] {"@"}},
    {"/", new string[1] {"/"}},
    {"!", new string[1] {"!"}},
    {"！", new string[1] {"!"}},
    {"?", new string[1] {"?"}},
    {"？", new string[1] {"?"}},
    {"\"", new string[1] {"\""}},
    {"#", new string[1] {"#"}},
    {"$", new string[1] {"$"}},
    {"%", new string[1] {"%"}},
    {"&", new string[1] {"&"}},
    {"\'", new string[1] {"\'"}},
    {"(", new string[1] {"("}},
    {")", new string[1] {")"}},
    {"=", new string[1] {"="}},
    {"~", new string[1] {"~"}},
    {"|", new string[1] {"|"}},
    {"`", new string[1] {"`"}},
    {"{", new string[1] {"{"}},
    {"}", new string[1] {"}"}},
    {"+", new string[1] {"+"}},
    {"*", new string[1] {"*"}},
    {"<", new string[1] {"<"}},
    {">", new string[1] {">"}},
    {"_", new string[1] {"_"}},
  };

  /// JIS かな入力で2打鍵必要なもののマッピング
  private static Dictionary<string, string[]> jisKanaMap = new Dictionary<string, string[]> {
    {"が", new string[2] {"か","゛"}},
    {"ぎ", new string[2] {"き","゛"}},
    {"ぐ", new string[2] {"く","゛"}},
    {"げ", new string[2] {"け","゛"}},
    {"ご", new string[2] {"こ","゛"}},
    {"ざ", new string[2] {"さ","゛"}},
    {"じ", new string[2] {"し","゛"}},
    {"ず", new string[2] {"す","゛"}},
    {"ぜ", new string[2] {"せ","゛"}},
    {"ぞ", new string[2] {"そ","゛"}},
    {"だ", new string[2] {"た","゛"}},
    {"ぢ", new string[2] {"ち","゛"}},
    {"づ", new string[2] {"つ","゛"}},
    {"で", new string[2] {"て","゛"}},
    {"ど", new string[2] {"と","゛"}},
    {"ば", new string[2] {"は","゛"}},
    {"び", new string[2] {"ひ","゛"}},
    {"ぶ", new string[2] {"ふ","゛"}},
    {"べ", new string[2] {"へ","゛"}},
    {"ぼ", new string[2] {"ほ","゛"}},
    {"ぱ", new string[2] {"は","゜"}},
    {"ぴ", new string[2] {"ひ","゜"}},
    {"ぷ", new string[2] {"ふ","゜"}},
    {"ぺ", new string[2] {"へ","゜"}},
    {"ぽ", new string[2] {"ほ","゜"}},
  };

  // 原文と読み方のセット
  private static Dictionary<int, List<(string originSentence, string typeSentence)>> wordSetDict = new Dictionary<int, List<(string originSentence, string typeSentence)>>();

  // データセット名
  // リザルト画面に表示する用として使いたい
  public static string DataSetName
  {
    private set;
    get;
  }

  /// <summary>
  /// ひらがな文をパースして、判定を作成
  /// <param name="sentenceHiragana">パースされるひらがな文字列</param>
  /// <returns>判定器</returns>
  /// </summary>
  private static List<List<string>> ConstructTypeSentence(string sentenceHiragana)
  {
    int i = 0;
    string uni = "";
    string bi = "";
    string tri = "";
    var judge = new List<List<string>>();
    while (i < sentenceHiragana.Length)
    {
      var validTypeList = new List<string>();
      uni = sentenceHiragana[i].ToString();
      bi = (i + 1 < sentenceHiragana.Length) ? sentenceHiragana.Substring(i, 2) : "";
      tri = (i + 2 < sentenceHiragana.Length) ? sentenceHiragana.Substring(i, 3) : "";
      if (romanTypeMap.ContainsKey(tri))
      {
        validTypeList = romanTypeMap[tri].ToList();
        i += 3;
      }
      else if (romanTypeMap.ContainsKey(bi))
      {
        validTypeList = romanTypeMap[bi].ToList();
        i += 2;
      }
      else
      {
        validTypeList = romanTypeMap[uni].ToList();
        // 文末「ん」の処理
        if (uni.Equals("ん") && sentenceHiragana.Length - 1 == i)
        {
          validTypeList.Remove("n");
        }
        i++;
      }
      judge.Add(validTypeList);
    }
    return judge;
  }

  /// <summary>
  /// ひらがな文をパースして、JIS かな用の判定を作成
  /// <param name="sentenceHiragana">パースされるひらがな文字列</param>
  /// <returns>判定器</returns>
  /// </summary>
  private static List<List<string>> ConstructJISKanaTypeSentence(string sentenceHiragana)
  {
    var judge = new List<List<string>>();
    for (int i = 0; i < sentenceHiragana.Length; ++i)
    {
      var hiragana = sentenceHiragana[i].ToString();
      if (jisKanaMap.ContainsKey(hiragana))
      {
        var mappingList = jisKanaMap[hiragana].ToList();
        var validTypeList1 = new List<string> { mappingList[0] };
        var validTypeList2 = new List<string> { mappingList[1] };
        judge.Add(validTypeList1);
        judge.Add(validTypeList2);
      }
      else
      {
        var validTypeList = new List<string> { sentenceHiragana[i].ToString() };
        judge.Add(validTypeList);
      }
    }
    return judge;
  }


  /// <summary>
  /// タイピング表示に必要な原文、ひらがな読みの文と、判定器を生成
  /// <returns>(原文, ひらがな文, タイピング判定器)</returns>
  /// </summary>
  public (bool isGenerateSuccess, string originSentence, string typeSentence, List<List<string>> typeJudge) Generate()
  {
    bool isOK = false;
    const int retryLimit = 100;
    int loopCount = 0;
    string originSentenceStr = "";
    string typeSentenceStr = "";
    var hiraganaSeparated = new List<string>();
    var retTypeJudge = new List<List<string>>();
    while (!isOK && loopCount < retryLimit)
    {
      // ワードデータセットに欠陥がある可能性も含めて try で動かす
      try
      {
        int r1 = UnityEngine.Random.Range(0, wordSetDict[0].Count);
        string tmpOriginSentenceStr = wordSetDict[0][r1].originSentence;
        string tmpTypeSentenceStr = wordSetDict[0][r1].typeSentence;
        foreach (var key in wordSetDict.Keys)
        {
          string replaceStr = "{" + key.ToString() + "}";
          int r2 = UnityEngine.Random.Range(0, wordSetDict[key].Count);
          tmpOriginSentenceStr = tmpOriginSentenceStr.Replace(replaceStr, wordSetDict[key][r2].originSentence);
          tmpTypeSentenceStr = tmpTypeSentenceStr.Replace(replaceStr, wordSetDict[key][r2].typeSentence);
        }
        // 一定文字数に収まっていることをチェック
        // 短かったり長かったりする場合は再度生成
        if (minLength <= tmpTypeSentenceStr.Length && tmpTypeSentenceStr.Length <= maxLength)
        {
          originSentenceStr = tmpOriginSentenceStr;
          typeSentenceStr = tmpTypeSentenceStr;
          retTypeJudge = (ConfigScript.InputMode == 0) ? ConstructTypeSentence(typeSentenceStr) : ConstructJISKanaTypeSentence(typeSentenceStr);
          isOK = true;
        }
      }
      catch
      {
        isOK = false;
      }
    }
    return (isOK, originSentenceStr, typeSentenceStr, retTypeJudge);
  }

  /// <summary>
  /// AssetBundle の読み込み
  /// <param name="callback">callback 関数</param>
  /// </summary>
  public IEnumerator LoadAssetBundle(UnityAction callback)
  {
    var networkState = Application.internetReachability;
    // すでに AssetBundle が読み込まれているか、
    // そうでないときにネットワークに接続していないときはリクエストを送信しない
    // ネットワーク接続していないときは何かしらエラーを出すとよさそう
    if (abData != null)
    {
      callback();
      yield break;
    }

    // WebGL 時は WebRequest によって AssetBundle を取得
#if UNITY_WEBGL && !UNITY_EDITOR
		if (networkState == NetworkReachability.NotReachable){
			Debug.Log("ネットワークに接続していません");
			callback();
			yield break;
		}
		UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(ABPath);
		yield return request.SendWebRequest();
		if (request.isNetworkError || request.isHttpError){
			Debug.LogError(request.error);
		}
		else {
			abData = DownloadHandlerAssetBundle.GetContent(request);
			Debug.Log("load successfully");
		}
#else
    abData = AssetBundle.LoadFromFile(ABPathLocal);
    if (abData == null)
    {
      Debug.Log("Error: AssetBundle Load failed");
    }
#endif

    callback();
    yield break;
  }

  /// <summary>
  /// ワードセットのデータを読み込む
  /// <param name="dataName">データセットのファイル名</param>
  /// <returns>読み込みが成功すれば true、さもなくば false</returns>
  /// </summary>
  public bool LoadSentenceData(string dataName)
  {
    if (abData == null)
    {
      return false;
    }
    try
    {
      wordSetDict = new Dictionary<int, List<(string originSentence, string typeSentence)>>();
      var jsonStr = abData.LoadAsset<TextAsset>(dataName).ToString();
      var problemData = JsonUtility.FromJson<SentenceData>(jsonStr);
      DataSetName = problemData.sentenceDatasetScreenName;
      lang = langMap[problemData.lang];
      foreach (var word in problemData.words)
      {
        var wordSection = word.wordSection;
        var wordInfo = (word.sentence, word.typeString);
        if (wordSetDict.ContainsKey(wordSection))
        {
          wordSetDict[wordSection].Add(wordInfo);
        }
        else
        {
          wordSetDict[wordSection] = new List<(string, string)>() { wordInfo };
        }
      }
    }
    catch (Exception e)
    {
      Debug.Log(e);
      return false;
    }
    return true;
  }
}

[Serializable]
public class SentenceData
{
  public string lang;
  public string sentenceDatasetName;
  public string sentenceDatasetScreenName;
  public Word[] words;
}

[Serializable]
public class Word
{
  public int wordSection;
  public string sentence;
  public string typeString;
}