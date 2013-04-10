using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.Specialized;

// The data model defined by this file serves as a representative example of a strongly-typed
// model that supports notification when members are added, removed, or modified.  The property
// names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs.

namespace BricksStyle.Data
{
    /// <summary>
    /// Base class for <see cref="SampleDataItem"/> and <see cref="SampleDataGroup"/> that
    /// defines properties common to both.
    /// </summary>
    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class SampleDataCommon : BricksStyle.Common.BindableBase
    {
        private static Uri _baseUri = new Uri("ms-appx:///");

        public SampleDataCommon(String uniqueId, String title, String subtitle, String imagePath, String description)
        {
            this._uniqueId = uniqueId;
            this._title = title;
            this._subtitle = subtitle;
            this._description = description;
            this._imagePath = imagePath;
        }

        private string _uniqueId = string.Empty;
        public string UniqueId
        {
            get { return this._uniqueId; }
            set { this.SetProperty(ref this._uniqueId, value); }
        }

        private string _title = string.Empty;
        public string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref this._title, value); }
        }

        private string _subtitle = string.Empty;
        public string Subtitle
        {
            get { return this._subtitle; }
            set { this.SetProperty(ref this._subtitle, value); }
        }

        private string _description = string.Empty;
        public string Description
        {
            get { return this._description; }
            set { this.SetProperty(ref this._description, value); }
        }

        private ImageSource _image = null;
        private String _imagePath = null;
        public ImageSource Image
        {
            get
            {
                if (this._image == null && this._imagePath != null)
                {
                    this._image = new BitmapImage(new Uri(SampleDataCommon._baseUri, this._imagePath));
                }
                return this._image;
            }

            set
            {
                this._imagePath = null;
                this.SetProperty(ref this._image, value);
            }
        }

        public void SetImage(String path)
        {
            this._image = null;
            this._imagePath = path;
            this.OnPropertyChanged("Image");
        }

        public override string ToString()
        {
            return this.Title;
        }
    }

    /// <summary>
    /// Generic item data model.
    /// </summary>
    public class SampleDataItem : SampleDataCommon
    {
        public SampleDataItem(String uniqueId, String title, String subtitle, String imagePath, String description, String content, int colSpan, int rowSpan, SampleDataGroup group)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            this._colSpan = colSpan;
            this._rowSpan = rowSpan;
            this._content = content;
            this._group = group;
        }

        private string _content = string.Empty;
        public string Content
        {
            get { return this._content; }
            set { this.SetProperty(ref this._content, value); }
        }

        private int _rowSpan = 1;
        public int RowSpan
        {
            get { return this._rowSpan; }
            set { this.SetProperty(ref this._rowSpan, value); }
        }

        private int _colSpan = 1;
        public int ColSpan
        {
            get { return this._colSpan; }
            set { this.SetProperty(ref this._colSpan, value); }
        }


        private SampleDataGroup _group;
        public SampleDataGroup Group
        {
            get { return this._group; }
            set { this.SetProperty(ref this._group, value); }
        }
    }

    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class SampleDataGroup : SampleDataCommon
    {
        public SampleDataGroup(String uniqueId, String title, String subtitle, String imagePath, String description)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            Items.CollectionChanged += ItemsCollectionChanged;
        }

        private void ItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Provides a subset of the full items collection to bind to from a GroupedItemsPage
            // for two reasons: GridView will not virtualize large items collections, and it
            // improves the user experience when browsing through groups with large numbers of
            // items.
            //
            // A maximum of 12 items are displayed because it results in filled grid columns
            // whether there are 1, 2, 3, 4, or 6 rows displayed

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex,Items[e.NewStartingIndex]);
                        if (TopItems.Count > 12)
                        {
                            TopItems.RemoveAt(12);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    if (e.OldStartingIndex < 12 && e.NewStartingIndex < 12)
                    {
                        TopItems.Move(e.OldStartingIndex, e.NewStartingIndex);
                    }
                    else if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        TopItems.Add(Items[11]);
                    }
                    else if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
                        TopItems.RemoveAt(12);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        if (Items.Count >= 12)
                        {
                            TopItems.Add(Items[11]);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems[e.OldStartingIndex] = Items[e.OldStartingIndex];
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    TopItems.Clear();
                    while (TopItems.Count < Items.Count && TopItems.Count < 12)
                    {
                        TopItems.Add(Items[TopItems.Count]);
                    }
                    break;
            }
        }

        private ObservableCollection<SampleDataItem> _items = new ObservableCollection<SampleDataItem>();
        public ObservableCollection<SampleDataItem> Items
        {
            get { return this._items; }
        }

        private ObservableCollection<SampleDataItem> _topItem = new ObservableCollection<SampleDataItem>();
        public ObservableCollection<SampleDataItem> TopItems
        {
            get {return this._topItem; }
        }
    }

    /// <summary>
    /// Creates a collection of groups and items with hard-coded content.
    /// 
    /// SampleDataSource initializes with placeholder data rather than live production
    /// data so that sample data is provided at both design-time and run-time.
    /// </summary>
    public sealed class SampleDataSource
    {
        private static SampleDataSource _sampleDataSource = new SampleDataSource();

        private ObservableCollection<SampleDataGroup> _allGroups = new ObservableCollection<SampleDataGroup>();
        public ObservableCollection<SampleDataGroup> AllGroups
        {
            get { return this._allGroups; }
        }

        public static IEnumerable<SampleDataGroup> GetGroups(string uniqueId)
        {
            if (!uniqueId.Equals("AllGroups")) throw new ArgumentException("Only 'AllGroups' is supported as a collection of groups");
            
            return _sampleDataSource.AllGroups;
        }

        public static SampleDataGroup GetGroup(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.AllGroups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static SampleDataItem GetItem(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.AllGroups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

       

        public SampleDataSource()
        {
            String ITEM_CONTENT = String.Format("Item Content: {0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}",
                        "Curabitur class aliquam vestibulum nam curae maecenas sed integer cras phasellus suspendisse quisque donec dis praesent accumsan bibendum pellentesque condimentum adipiscing etiam consequat vivamus dictumst aliquam duis convallis scelerisque est parturient ullamcorper aliquet fusce suspendisse nunc hac eleifend amet blandit facilisi condimentum commodo scelerisque faucibus aenean ullamcorper ante mauris dignissim consectetuer nullam lorem vestibulum habitant conubia elementum pellentesque morbi facilisis arcu sollicitudin diam cubilia aptent vestibulum auctor eget dapibus pellentesque inceptos leo egestas interdum nulla consectetuer suspendisse adipiscing pellentesque proin lobortis sollicitudin augue elit mus congue fermentum parturient fringilla euismod feugiat");

            var group1 = new SampleDataGroup("Group-1",
                 "Gardening Art",
                 "Gardening Art",
                 "Assets/10.jpg",
                 "Garden design is considered to be an art in most cultures, distinguished from gardening, which generally means garden maintenance. Garden design can include different themes such as perennial, butterfly, wildlife, Japanese, water, tropical, or shade gardens. In Japan, Samurai and Zen monks were often required to build decorative gardens or practice related skills like flower arrangement.");

            group1.Items.Add(new SampleDataItem("Big-Group-1-Item1",
                 "Gardening",
                 "Gardening",
                 "Assets/11.jpg",
                 "Gardening is the practice of growing and cultivating plants as part of horticulture. In gardens, ornamental plants are often grown for their flowers, foliage, or overall appearance; useful plants, such as root vegetables, leaf vegetables, fruits, and herbs, are grown for consumption, for use as dyes, or for medicinal or cosmetic use.",
                 "Gardening is the practice of growing and cultivating plants as part of horticulture. In gardens, ornamental plants are often grown for their flowers, foliage, or overall appearance; useful plants, such as root vegetables, leaf vegetables, fruits, and herbs, are grown for consumption, for use as dyes, or for medicinal or cosmetic use. A gardener is someone who practices gardening, either professionally or as a hobby. Gardening is considered to be a relaxing activity for many people.\n\nGardening ranges in scale from fruit orchards, to long boulevard plantings with one or more different types of shrubs, trees and herbaceous plants, to residential yards including lawns and foundation plantings, to plants in large or small containers grown inside or outside. Gardening may be very specialized, with only one type of plant grown, or involve a large number of different plants in mixed plantings. It involves an active participation in the growing of plants, and tends to be labor intensive, which differentiates it from farming or forestry.",
                 79,
                 49,
                 group1));

            group1.Items.Add(new SampleDataItem("Small-Group-1-Item2",
                 "History",
                 "History",
                 "Assets/12.jpg",
                 "Forest gardening, a plant-based food production system, is the world's oldest form of gardening.[1] Forest gardens originated in prehistoric times along jungle-clad river banks and in the wet foothills of monsoon regions. In the gradual process of families improving their immediate environment.",
                 "Forest gardening, a plant-based food production system, is the world's oldest form of gardening.[1] Forest gardens originated in prehistoric times along jungle-clad river banks and in the wet foothills of monsoon regions. In the gradual process of families improving their immediate environment, useful tree and vine species were identified, protected and improved whilst undesirable species were eliminated. Eventually foreign species were also selected and incorporated into the gardens.\n\nAfter the emergence of the first civilizations, wealthy individuals began to create gardens for purely aesthetic purposes. Egyptian tomb paintings from around 1500 BC provide some of the earliest physical evidence of ornamental horticulture and landscape design; they depict lotus ponds surrounded by symmetrical rows of acacias and palms. Ornamental gardens were known in ancient times, a famous example being the Hanging Gardens of Babylon, while ancient Rome had dozens of gardens. The Hanging Gardens of Babylon are a World Heritage Site and one of the Seven Wonders of the Ancient World.",
                 53,
                 49,
                 group1));

            group1.Items.Add(new SampleDataItem("Big-Group-1-Item3",
                 "Accessories",
                 "Accessories",
                 "Assets/13.jpg",
                 "Gardening may be performed at a professional level, a hobby, or for therapeutic reasons. There is a wide range of accessories available in the market for both the professional gardener and the amateur to exercise their creativity. These accessories can help decorate all the different areas of gardens such as walk ways and raised beds, and any other area.",
                 "Gardening may be performed at a professional level, a hobby, or for therapeutic reasons. There is a wide range of accessories available in the market for both the professional gardener and the amateur to exercise their creativity. These accessories can help decorate all the different areas of gardens such as walk ways and raised beds, and any other area.\n\nLocation, size, budget are all characteristics to be considered when choosing accessories to improve a garden's deco. Accessories are made of different materials such as copper, stone, wood, bamboo, metal, stainless steel, clay, stained glass, concrete, iron, and the weather where the garden is located will determine which material works best to ensure accessories last long.\n\nA garden's decoration with the appropriate accessories also adds personality and beauty, and depending on the situation, the decoration chosen will provide functionality to the garden. Paths for instance are functional for the maintenance of the garden, and can be somehow decorated using different materials such as pine needles, wood chips, fieldstone, or bricks. Also, backdrops include walls, fences, and hedges which are intended to provide privacy. Moreover, they hide unsightly areas and also emphasize desired views.",
                 79,
                 49,
                 group1));

            group1.Items.Add(new SampleDataItem("Big-Group-1-Item4",
                 "Comparison with farming",
                 "Comparison with farming",
                 "Assets/14.jpg",
                 "Gardening for beauty is likely nearly as old as farming for food, however for most of history for the majority of people there was no real distinction since the need for food and other useful product trumped other concerns.",
                 "Gardening for beauty is likely nearly as old as farming for food, however for most of history for the majority of people there was no real distinction since the need for food and other useful product trumped other concerns. Small-scale, subsistence agriculture (called hoe-farming) is largely indistinguishable from gardening. A patch of potatoes grown by a Peruvian peasant or an Irish smallholder for personal use could be described as either a garden or a farm. Gardening for average people evolved as a separate discipline, more concerned with esthetics and recreation, under the influence of the pleasure gardens of the wealthy. Meanwhile farming has evolved (in developed countries) in the direction of commercialization, economics of scale, and monocropping.\n\nIn respect to its food producing purpose, gardening is distinguished from farming chiefly by scale and intent. Farming occurs on a larger scale, and with the production of salable goods as a major motivation. Gardening is done on a smaller scale, primarily for pleasure and to produce goods for the gardener's own family or community. There is some overlap between the terms, particularly in that some moderate-sized vegetable growing concerns, often called market gardening, can fit in either category.",
                 79,
                 49,
                 group1));

            group1.Items.Add(new SampleDataItem("Big-Group-1-Item5",
                 "Social Aspects",
                 "Social Aspects",
                 "Assets/15.jpg",
                 "People often surround their house and garden with a hedge. Common hedge plants are privet, hawthorn, beech, yew, leyland cypress, hemlock, arborvitae, barberry, box, holly, oleander, forsythia and lavender. The idea of open gardens without hedges may be distasteful to those who enjoy privacy. This may have an advantage to local wildlife by providing a habitat for birds, animals, and wild plants.",
                 "People can express their political or social views in gardens, intentionally or not. The lawn vs. garden issue is played out in urban planning as the debate over the land ethic that is to determine urban land use and whether hyper hygienist bylaws (e.g. weed control) should apply, or whether land should generally be allowed to exist in its natural wild state. In a famous Canadian Charter of Rights case, Sandra Bell vs. City of Toronto, 1997 the right to cultivate all native species, even most varieties deemed noxious or allergenic, was upheld as part of the right of free expression.\n\nCommunity gardening comprises a wide variety of approaches to sharing land and gardens.People often surround their house and garden with a hedge. Common hedge plants are privet, hawthorn, beech, yew, leyland cypress, hemlock, arborvitae, barberry, box, holly, oleander, forsythia and lavender. The idea of open gardens without hedges may be distasteful to those who enjoy privacy.This may have an advantage to local wildlife by providing a habitat for birds, animals, and wild plants.The Slow Food movement has sought in some countries to add an edible school yard and garden classrooms to schools, e.g. in Fergus, Ontario, where these were added to a public school to augment the kitchen classroom. Garden sharing, where urban landowners allow gardeners to grow on their property in exchange for a share of the harvest, is associated with the desire to control the quality of one's food, and reconnect with soil and community. \n\nIn US and British usage, the production of ornamental plantings around buildings is called landscaping, landscape maintenance or grounds keeping, while international usage uses the term gardening for these same activities.\n\nAlso gaining popularity is the concept of Green Gardening which involves growing plants using organic fertilizers and pesticides so that the gardening process - or the flowers and fruits produced thereby - doesn't adversely affect the environment or people's health in any manner.",
                 79,
                 49,
                 group1));

            group1.Items.Add(new SampleDataItem("Small-Group-1-Item6",
                 "Garden Pests",
                 "Garden Pests",
                 "Assets/16.jpg",
                 "A garden pest is generally an insect, plant, or animal that engages in activity that the gardener considers undesirable. It may crowd out desirable plants, disturb soil, stunt the growth of young seedlings, steal or damage fruit, or otherwise kill plants, hamper their growth, damage their appearance, or reduce the quality of the edible or ornamental portions of the plant.",
                 "A garden pest is generally an insect, plant, or animal that engages in activity that the gardener considers undesirable. It may crowd out desirable plants, disturb soil, stunt the growth of young seedlings, steal or damage fruit, or otherwise kill plants, hamper their growth, damage their appearance, or reduce the quality of the edible or ornamental portions of the plant.\n\nBecause gardeners may have different goals, organisms considered garden pests vary from gardener to gardener. For example, Tropaeolum speciosum, while beautiful, can be considered a pest if it seeds and starts to grow where it is not wanted. As the root is well below ground, pulling it up does not remove it: it simply grows again and becomes what may be considered a pest. As another example, in lawns, moss can become dominant and be impossible to eradicate. In some lawns, lichens, especially very damp lawn lichens such as Peltigera lactucfolia and P. membranacea, can become difficult to control and be considered pests. Despite this, aphids, spider mites, slugs, snails, ants, birds, and even cats are commonly considered to be garden pests.\n\nThere are many ways to remove unwanted pests from a garden. The techniques vary depending on the pest, the gardener's goals, and the gardener's philosophy. For example, snails may be dealt with through the use of a chemical pesticide, an organic pesticide, hand-picking, barriers, or simply growing snail-resistant plants.",
                 53,
                 49,
                 group1));
            
            this.AllGroups.Add(group1);

             var group2 = new SampleDataGroup("Group-2",
                 "Perfect Gardening",
                 "Perfect Gardening",
                 "Assets/20.jpg",
                 "Gardening ranges in scale from fruit orchards, to long boulevard plantings with one or more different types of shrubs, trees and herbaceous plants, to residential yards including lawns and foundation plantings, to plants in large or small containers grown inside or outside. Gardening may be very specialized.");

             group2.Items.Add(new SampleDataItem("Big-Group-2-Item1",
                 "The Dirt on Dirt",
                 "The Dirt on Dirt",
                 "Assets/21.jpg",
                 "Most produce from the grocery store doesn't have the nutrition that it used to have, so more and more people are becoming interested in growing their own food nutrient-dense fruits, vegetables, grains, nuts and seeds. Building Soils Naturally shows gardeners how to grow more nutritious food and have more healthy, pest-resistant flowers and ornamental plants.",
                 "Most produce from the grocery store doesn't have the nutrition that it used to have, so more and more people are becoming interested in growing their own food nutrient-dense fruits, vegetables, grains, nuts and seeds. Building Soils Naturally shows gardeners how to grow more nutritious food and have more healthy, pest-resistant flowers and ornamental plants. Whether experienced or just getting started, gardeners are likely to encounter some perplexing (and common) setbacks: Certain fruit and vegetable plants that don't produce as expected, Ornamental plants that fail to thrive, And plant predators that chew plants to the ground. All of these issues point to plants that aren't at optimum health. So, what could be wrong? Plants may be lacking in proper nutrition, missing beneficial microorganism companions, or short of energy to reach their full nutrient-dense potential. The advice most often given is start with the soil.",
                 79,
                 49,
                 group2));

             group2.Items.Add(new SampleDataItem("Small-Group-2-Item2",
                 "Forest Gardening",
                 "Forest Gardening",
                 "Assets/22.jpg",
				 "Forest gardens are probably the world's oldest form of land use and most resilient agroecosystem. They originated in prehistoric times along jungle-clad river banks and in the wet foothills of monsoon regions. In the gradual process of families improving their immediate environment, useful tree and vine species were identified, protected and improved whilst undesirable species were eliminated.",
                 "Forest gardens are probably the world's oldest form of land use and most resilient agroecosystem. They originated in prehistoric times along jungle-clad river banks and in the wet foothills of monsoon regions. In the gradual process of families improving their immediate environment, useful tree and vine species were identified, protected and improved whilst undesirable species were eliminated. Eventually superior foreign species were selected and incorporated into the gardens.\n\nForest gardens are still common in the tropics and known by various names such as: home gardens in Kerala in South India, Nepal, Zambia, Zimbabwe and Tanzania; Kandyan forest gardens in Sri Lanka huertos familiares, the family orchards of Mexico and pekarangan, the gardens of complete design, in Java. These are also called agroforests and, where the wood components are short statured, the term shrub garden is employed. Forest gardens have been shown to be a significant source of income and food security for local populations. \n\nRobert Hart adapted forest gardening for the UK's temperate climate during the early 1960s. His theories were later developed by Martin Crawford from the Agroforestry Research Trust and various permaculturalists such as Graham Bell, Patrick Whitefield, Dave Jacke and Geoff Lawton.",
                 53,
                 49,
                 group2));

             group2.Items.Add(new SampleDataItem("Big-Group-2-Item3",
                 "Garden Design",
                 "Garden Design",
                 "Assets/23.jpg",
                 "Garden design is the art and process of designing and creating plans for layout and planting of gardens and landscapes. Garden design may be done by the garden owner themselves, or by professionals of varying levels of experience and expertise.",
                 "Garden design is the art and process of designing and creating plans for layout and planting of gardens and landscapes. Garden design may be done by the garden owner themselves, or by professionals of varying levels of experience and expertise. Most professional garden designers have some training in horticulture and the principles of design, and some are also landscape architects, a more formal level of training that usually requires an advanced degree and often a state license. Amateur gardeners may also attain a high level of experience from extensive hours working in their own gardens, through casual study, serious study in Master Gardener Programs, or by joining gardening clubs.",
                 79,
                 49,
                 group2));

             group2.Items.Add(new SampleDataItem("Big-Group-2-Item4",
                 "Islamic Garden",
                 "Islamic Garden",
                 "Assets/24.jpg",
                 "Traditionally, an Islamic garden is a cool place of rest and reflection, and a reminder of paradise. The Qur'an has many references to gardens, and the garden is used as an earthly analogue for the life in paradise which is promised to believers.",
                 "Traditionally, an Islamic garden is a cool place of rest and reflection, and a reminder of paradise. The Qur'an has many references to gardens, and the garden is used as an earthly analogue for the life in paradise which is promised to believers:\n\nAllah has promised to the believing men and the believing women gardens, beneath which rivers flow, to abide in them, and goodly dwellings in gardens of perpetual abode; and best of all is Allah's goodly pleasure; that is the grand achievement (Qur'an 9.72)\n\nThere are surviving formal Islamic gardens in a wide zone extending from Spain and Morocco in the west to India in the east. Famous Islamic gardens include those of the Taj Mahal in India and the Generalife and Alhambra in Spain.\n\nThe general theme of a traditional Islamic garden is water and shade, not surprisingly since Islam came from and generally spread in a hot and arid climate. Unlike English gardens, which are often designed for walking, Islamic gardens are intended for rest and contemplation. For this reason, Islamic gardens usually include places for sitting.",
                 79,
                 49,
                 group2));

             group2.Items.Add(new SampleDataItem("Big-Group-2-Item5",
                 "Water Garden",
                 "Water Garden",
                 "Assets/25.jpg",
                 "Water gardens, also known as aquatic gardens, are a type of man-made water feature. A water garden is defined as any interior or exterior landscape or architectural element whose primarily purpose is to house, display, or propagate a particular species or variety of aquatic plant.",
                 "Water gardens, also known as aquatic gardens, are a type of man-made water feature. A water garden is defined as any interior or exterior landscape or architectural element whose primarily purpose is to house, display, or propagate a particular species or variety of aquatic plant. Although a water garden's primary focus is on plants, they will sometimes also house ornamental fish, in which case the feature will be a fish pond.\n\nAlthough water gardens can be almost any size or depth, they are typically small and relatively shallow, generally less than twenty inches in depth. This is because most aquatic plants are depth sensitive and require a specific water depth in order to thrive. The particular species inhabiting each water garden will ultimately determine the actual surface area and depth required.",
                 79,
                 49,
                 group2));

             group2.Items.Add(new SampleDataItem("Small-Group-2-Item6",
                 "Community Gardening",
                 "Community Gardening",
                 "Assets/26.jpg",
                 "Community gardens provide fresh produce and plants as well as satisfying labor, neighborhood improvement, sense of community and connection to the environment.",
                 "Community gardens vary widely throughout the world. In North America, community gardens range from familiar victory garden areas where people grow small plots of vegetables, to large greening projects to preserve natural areas, to tiny street beautification planters on urban street corners. Some grow only flowers, others are nurtured communally and their bounty shared. There are even non-profits in many major cities that offer assistance to low-income families, children groups, and community organizations by helping them develop and grow their own gardens. In the UK and the rest of Europe, closely related allotment gardens can have dozens of plots, each measuring hundreds of square meters and rented by the same family for generations. In the developing world, commonly held land for small gardens is a familiar part of the landscape, even in urban areas, where they may function as mini-truck farms.",
                 53,
                 49,
                 group2));
            
            this.AllGroups.Add(group2);
			
           
        }
    }
}
