import React from 'react';
import { Sidebar } from './../sidebar';
import { shallow, mount, configure } from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';
import Site from '../../../lib/site';
import renderer from 'react-test-renderer';

configure({ adapter: new Adapter() })

function setup(props, isShallow = true) {
    const wrap = isShallow ? (component) => shallow(component) : (component) => mount(component)
    return wrap(<Sidebar {...props} />)
}

describe('Sidebar', () => {
    describe('render', () => {
        const mockMenu = jest.fn()
        let wrapper = null
        beforeEach(() => {
            wrapper = setup(Sidebar)
            wrapper.instance().getMenu = mockMenu
        })

        it('menu is empty', () => {
            // Arrange
            mockMenu.mockReturnValueOnce([])
            wrapper.instance().forceUpdate()

            // Act
            const renderOutput = renderer.create(wrapper).toJSON()

            // Assert
            expect(renderOutput).toMatchSnapshot()
        })

        it('menu is not empty', () => {
            // Arrange
            mockMenu.mockReturnValueOnce([
                {
                    SubMenus: [
                        {
                            Name: 'subMenu1',
                            Url: '/subMenu1'
                        },
                        {
                            Name: 'subMenu2',
                            Url: '/subMenu2'
                        }
                    ]
                }
            ])
            wrapper.instance().forceUpdate()
            
            // Act
            const renderOutput = renderer.create(wrapper).toJSON()

            // Assert
            expect(renderOutput).toMatchSnapshot()
        })
    })

    describe('behavior', () => {
        it('should call onChange prop with correct url when onChange method is called', () => {
            // Arrange
            const props = {
                onChange: jest.fn()
            }
            const sidebar = new Sidebar(props)
            const fakeData = {
                event: {
                    preventDefault: jest.fn()
                },
                url: 'any'
            }

            // Act
            sidebar.onChange(fakeData.event, fakeData.url)

            // Assert
            expect(props.onChange).toHaveBeenCalledWith(Site.resolveClientUrl(fakeData.url))
        })
    })
})
