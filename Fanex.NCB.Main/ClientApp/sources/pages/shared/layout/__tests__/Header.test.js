import React from 'react';
import Header from './../header/index';
import { shallow } from 'enzyme';

describe('Header', () => {
    describe('render', () => {
        it('sidebar is expanded', () => {
            // Arrange
            const props = {
                isSidebarExpanded: false
            };

            // Act
            const renderOutput = shallow(<Header {...props} />).html();

            // Assert
            expect(renderOutput).toMatchSnapshot();
        });

        it('sidebar is not expanded', () => {
            // Arrange
            const props = {
                isSidebarExpanded: true
            };

            // Act
            const renderOutput = shallow(<Header {...props} />).html();

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
            const renderOutput = shallow(<Header {...props} />).html();

            // Assert
            expect(renderOutput).toMatchSnapshot();
        });
    });

    describe('behavior', () => {
        it('should call collapseExpandHandler prop when click', () => {
            // Arrange
            const props = {
                collapseExpandHandler: jest.fn()
            };
            const sidebar = shallow(<Header {...props} />);

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