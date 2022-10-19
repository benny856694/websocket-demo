using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebsocketDemo
{
    public class FaceCaptureEvent
    {
        public string addr_name { get; set; }
        public string addr_no { get; set; }
        public string cap_time { get; set; }
        public Closeup_Pic closeup_pic { get; set; }
        public bool closeup_pic_flag { get; set; }
        public string cmd { get; set; }
        public string device_no { get; set; }
        public string device_sn { get; set; }
        public int is_realtime { get; set; }
        public Match match { get; set; }
        public int match_failed_reson { get; set; }
        public int match_result { get; set; }
        public bool overall_pic_flag { get; set; }
        public Person person { get; set; }
        public int sequence_no { get; set; }
        public string version { get; set; }
        public bool video_flag { get; set; }
    }

    public class Closeup_Pic
    {
        public string data { get; set; }
        public int face_height { get; set; }
        public int face_width { get; set; }
        public int face_x { get; set; }
        public int face_y { get; set; }
        public string format { get; set; }
    }

    public class Match
    {
        public string customer_text { get; set; }
        public string format { get; set; }
        public string image { get; set; }
        public bool is_encryption { get; set; }
        public string[] match_type { get; set; }
        public string origin { get; set; }
        public string person_attr { get; set; }
        public string person_id { get; set; }
        public string person_name { get; set; }
        public int person_role { get; set; }
        public int wg_card_id { get; set; }
    }

    public class Person
    {
        public int age { get; set; }
        public int face_quality { get; set; }
        public bool has_mask { get; set; }
        public string hat { get; set; }
        public int rotate_angle { get; set; }
        public string sex { get; set; }
        public float temperatur { get; set; }
        public int turn_angle { get; set; }
        public int wg_card_id { get; set; }
    }

}
