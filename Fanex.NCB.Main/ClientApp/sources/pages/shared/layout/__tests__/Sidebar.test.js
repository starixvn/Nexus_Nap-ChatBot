import React from 'react';
import Sidebar from './../sidebar/index';
import { shallow, mount, configure } from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';
import Site from '../../../lib/site';
import renderer from 'react-test-renderer';

configure({ adapter: new Adapter() });

function setup(props, isShallow = true) {
    if (isShallow) {
        return shallow(<Sidebar {...props} />);
    }
    return mount(<Sidebar {...props} />);
}

describe('Sidebar', () => {
    describe('render', () => {
        const mockMenu = jest.fn();
        let wrapper = null;
        beforeEach(() => {
            wrapper = setup();
            wrapper.instance().getMenu = mockMenu;
        });

        it('menu is empty', () => {
            // Arrange
            mockMenu.mockReturnValueOnce([]);
            wrapper.instance().forceUpdate();

            // Act
            const renderOutput = renderer.create(wrapper).toJSON();

            // Assert
            expect(renderOutput).toMatchSnapshot();
        });

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
            ]);
            wrapper.instance().forceUpdate();

            // Act
            const renderOutput = renderer.create(wrapper).toJSON();

            // Assert
            expect(renderOutput).toMatchSnapshot();
        });
    });

    describe('behavior', () => {
        it('should call componentDidMount', () => {
            const spy = jest.spyOn(Sidebar.prototype, 'componentDidMount');
            const component = shallow(
                <Sidebar />
            );

            expect(spy).toHaveBeenCalled();
        });
    });
});