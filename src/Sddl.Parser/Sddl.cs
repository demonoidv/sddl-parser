using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sddl.Parser
{
    public class Sddl
    {
        public string Raw { get; }

        public Sid Owner { get; }
        public Sid Group { get; }
        public Acl Dacl { get; }
        public Acl Sacl { get; }

        public Sddl(string sddl, SecurableObjectType type = SecurableObjectType.Unknown)
        {
            Raw = sddl;

            Dictionary<char, string> components = new Dictionary<char, string>();

            int i = 0;
            int idx = 0;
            int len = 0;

            while (i != -1)
            {
                i = sddl.IndexOf(DeliminatorToken, idx + 1);

                if (idx > 0)
                {
                    len = i > 0
                        ? i - idx - 2
                        : sddl.Length - (idx + 1);
                    components.Add(sddl[idx - 1], sddl.Substring(idx + 1, len));
                }

                idx = i;
            }

            if (components.TryGetValue(OwnerToken, out var owner))
            {
                Owner = new Sid(owner);
                components.Remove(OwnerToken);
            }

            if (components.TryGetValue(GroupToken, out var group))
            {
                Group = new Sid(group);
                components.Remove(GroupToken);
            }

            if (components.TryGetValue(DaclToken, out var dacl))
            {
                Dacl = new Acl(dacl, type);
                components.Remove(DaclToken);
            }

            if (components.TryGetValue(SaclToken, out var sacl))
            {
                Sacl = new Acl(sacl, type);
                components.Remove(SaclToken);
            }

            if (components.Any())
            {
                // ERROR Unknown components encountered.
            }
        }

        internal const char DeliminatorToken = ':';
        internal const char OwnerToken = 'O';
        internal const char GroupToken = 'G';
        internal const char DaclToken = 'D';
        internal const char SaclToken = 'S';

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            if (Owner != null)
                sb.AppendLine($"{nameof(Owner)}: {Owner.ToString()}");
            
            if (Group != null)
                sb.AppendLine($"{nameof(Group)}: {Group.ToString()}");

            if (Dacl != null)
            {
                sb.AppendLine($"{nameof(Dacl)}: ");
                sb.AppendLine(Format.Indent(Dacl.ToString()));
            }

            if (Sacl != null)
            {
                sb.AppendLine($"{nameof(Sacl)}: ");
                sb.AppendLine(Format.Indent(Dacl.ToString()));
            }

            return sb.ToString();
        }
    }
}