using System;
using System.Threading.Tasks;

namespace GBNsender {
    class Program {
        static async Task Main (string[] args) 
        {
            string[] message_pool = {
                "The paper explores the relationship between image complexity and the efficiency of various types of neural networks (NNs) in processing medical images. It highlights the importance of considering both model complexity and data complexity to analyze model performance.\r\n",
                "The versatility of the image complexity concept is illustrated through examples from medical imaging. For instance, in tumor diagnostics using MRI, the conventional separation of tumor and background is effective, while in myopathy diagnostics using ultrasound, fine-grained details are crucial despite high levels of visual noise.\r\n",
                "The paper does not provide a detailed methodological framework but suggests that future research should develop metrics capable of assessing both input complexity and the complexity introduced during processing within NNs. \r\n"
            };

            foreach(var message in message_pool)
            {
                var ctl = new Control("14:7d:da:6a:44:78", "ac:de:48:00:11:22");
                await ctl.Send(message);
                Console.WriteLine($"Message sent:'{message}'");
            }

            Console.ReadLine();
        }
    }
}
