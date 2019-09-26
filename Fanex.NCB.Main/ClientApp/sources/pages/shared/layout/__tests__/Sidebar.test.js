import React from 'react';
import Sidebar from './../sidebar/index';
import { shallow, mount, configure } from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';
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
            shallow(
                <Sidebar />
            );

            expect(spy).toHaveBeenCalled();
        });

        it('should call functions when render', () => {
            const wrapper = shallow(
                <Sidebar />
            );

            expect(wrapper.instance().getMenuClassName("test")).toEqual("icon-test");
            expect(wrapper.instance().getMenuClassName()).toEqual("");
            expect(wrapper.instance().isCurrent("url")).not.toBeTruthy();
        });

        it('render menu item', () => {
            const expected = (
                <li className="menu-item" key="1">
                    <a href="/url" className="link false ">
                        <span className="icon-test" />
                        <span className="text">test</span>
                    </a>
                </li >
            );

            const wrapper = shallow(
                <Sidebar />
            );
            const submenu = { id: 1, url: "url", name: "test" };

            const act = wrapper.instance().renderMenuItem(submenu);

            expect(act).toEqual(expected);
        });

        it('render menu listitem', () => {
            const result = (
                <li className="menu-item" key="1">
                    <a href="/url" className="link false ">
                        <span className="icon-test" />
                        <span className="text">test</span>
                    </a>
                </li >
            );

            let expected = [];
            expected.push(result);

            const wrapper = shallow(
                <Sidebar />
            );
            wrapper.setState({ menuItems: [{ id: 1, url: "url", name: "test" }] });

            const act = wrapper.instance().renderMenu();

            expect(act).toEqual(expected);
        });
    });
});