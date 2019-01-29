import React from 'react'
import renderer from 'react-test-renderer'
import Pager from '../Pager'

describe('Pager', () => {   
    describe('render', () => {
        it('component is rendered correctly', () => {
            // Arrange
            const props = {
                pageOption: {
                    PageIndex: 1,
                    PageSize: 5,
                    PageList: [5, 10, 20],
                    TotalItem: 10
                },
                onPaging: jest.fn()
            };

            // Act
            const component = renderer.create(<Pager {...props} />);
            const result = component.toJSON();

            // Assert
            expect(result).toMatchSnapshot();
        });

        it('component is hidden if paging is not available', () => {
            // Arrage
            const props = {
                pageOption: {
                    PageIndex: 1,
                    PageSize: 5,
                    PageList: [5, 10, 20],
                    TotalItem: 1
                },
                onPaging: jest.fn()
            };

            // Act
            const component = renderer.create(<Pager {...props} />);
            const result = component.toJSON();

            // Assert
            expect(result).toMatchSnapshot();
        });
    });

    describe("renderPageSizeSelection", () => {
      it("when called - create page size selection", () => {
          // Arrange
          let props = {
              goToPage: jest.fn()
          }
          let pager = new Pager(props);
          let pageList = [10, 20, 50];

          // Act
          const result = renderer.create(pager.renderPageSizeSelection(10, pageList)).toJSON();

          // Assert
          expect(result.children.length).toEqual(3);
          result.children.forEach((item, index) => {
            expect(item.props.value).toEqual(pageList[index]);
        });
      })  
    })

    describe("renderPageIcon", () => {
        it("when has class name disable", () => {
            // Arrange
            let pager = new Pager();

            // Act
            const result = renderer.create(pager.renderPageIcon("icon-class", true, () => {})).toJSON();

            // Assert
            expect(result.props.className).toEqual(expect.stringContaining("disable"));
        })

        it("when not has class name disable", () => {
            // Arrange
            let pager = new Pager();

            // Act
            const result = renderer.create(pager.renderPageIcon("icon-class", false, () => {})).toJSON();

            // Assert
            expect(result.props.className).toEqual(expect.not.stringContaining("disable"));
        })
    })

    describe("renderPageInfo", () => {
        it("when viewTo > total item", () => {
            // Arrange
            let pager = new Pager();

            // Act
            const result = renderer.create(pager.renderPageInfo(2, 10, 15)).toJSON();

            // Assert
            expect(result.children[0]).toEqual("Displaying 11 to 15 of 15 item(s)");
        })

        it("when viewTo <= total item", () => {
            // Arrange
            let pager = new Pager();

            // Act
            const result = renderer.create(pager.renderPageInfo(2, 10, 25)).toJSON();

            // Assert
            expect(result.children[0]).toEqual("Displaying 11 to 20 of 25 item(s)");
        })        
    })

    describe("renderPages", () => {
        it("when called - render span pages with active class at pageIndex", () => {
            // Arrange
            let pager = new Pager();

            // Act
            const result = renderer.create(pager.renderPages(2, 10, 3)).toJSON();

            // Assert
            expect(result.children.length).toEqual(3);
            expect(result.children[0].props.className).toEqual(expect.not.stringContaining("active"));
            expect(result.children[1].props.className).toEqual(expect.stringContaining("active"));
            expect(result.children[2].props.className).toEqual(expect.not.stringContaining("active"));
        })
    })

    describe("calculatePages", () => {
        it("when totalPage < 10", () => {
            // Arrange
            let pager = new Pager();

            // Act
            let result = pager.calculatePages(2, 5);

            // Assert
            expect(result).toEqual([1,2,3,4,5]);
        })

        it("when totalPage >= 10", () => {
            // Arrange
            let pager = new Pager();

            // Act
            let result = pager.calculatePages(10, 20);

            // Assert
            expect(result).toEqual([1,7,8,9,10,11,12,13,20]);
        })
    })

    describe('goToPage', () => {
        it('call onPaging', () => {
           // Arrange
           let props  = {
               onPaging: jest.fn()
           };
           let pager = new Pager(props);
           let pageIndex = 1;
           let pageSize = 10;

           // Act
           pager.goToPage(pageIndex, pageSize);

           // Assert
           expect(props.onPaging).toHaveBeenCalledWith(pageIndex, pageSize);
        });        
    });      
});