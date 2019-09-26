import React from 'react';
import Header from './../header/index';
import { shallow, mount, configure } from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';
import renderer from 'react-test-renderer';
import { Dropdown, DropdownToggle, DropdownMenu, DropdownItem } from 'reactstrap';

configure({ adapter: new Adapter() });

function setup(props, isShallow = true) {
    const wrap = isShallow ? (component) => shallow(component) : (component) => mount(component);
    return wrap(<Header {...props} />);
}

describe('Header', () => {
    describe('render', () => {
        it('sidebar is expanded', () => {
            // Arrange
            const props = {
                isSidebarExpanded: false
            };

            // Act
            const renderOutput = renderer.create(<Header {...props} />).toJSON();

            // Assert
            expect(renderOutput).toMatchSnapshot();
        });

        it('sidebar is not expanded', () => {
            // Arrange
            const props = {
                isSidebarExpanded: true
            };

            // Act
            const renderOutput = renderer.create(<Header {...props} />).toJSON();

            // Assert
            expect(renderOutput).toMatchSnapshot();
        });

        it('render without username', () => {
            spyOn(document, 'getElementById').and.callFake(function () {
                return [{
                    value: 'abc test'
                }];
            });

            const props = {
                isSidebarExpanded: true
            };

            // Act
            const renderOutput = renderer.create(<Header {...props} />).toJSON();

            // Assert
            expect(renderOutput).toMatchSnapshot();
        });

        it('render correct chil components', () => {
            const props = {
                isSidebarExpanded: true,
                collapseExpandHandler: jest.fn()
            };

            const wrapper = shallow(<Header {...props} />);

            expect(wrapper.find(Dropdown)).toHaveLength(1);
            expect(wrapper.find(DropdownToggle)).toHaveLength(1);
            expect(wrapper.find(DropdownMenu)).toHaveLength(1);
            expect(wrapper.find(DropdownItem)).toHaveLength(1);
        });
    });

    describe('behavior', () => {
        it('should call collapseExpandHandler prop when click', () => {
            // Arrange
            const props = {
                collapseExpandHandler: jest.fn()
            };
            const sidebar = new setup(props);

            // Act
            sidebar.find('div.line-wrapp').simulate('click');

            // Assert
            expect(props.collapseExpandHandler).toHaveBeenCalled();
        });

        it('when toggle, state dropdownOpen will change', () => {
            const props = {
                isSidebarExpanded: true,
                collapseExpandHandler: jest.fn()
            };

            const wrapper = shallow(<Header {...props} />);
            wrapper.setState({
                dropdownOpen: true, username: 'abc test'
            });
            wrapper.instance().toggle();
            expect(wrapper.state().dropdownOpen).toBe(false);
        });
    });
});