import React from 'react';
import { shallow, configure } from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';
import renderer from 'react-test-renderer';
import Content from './../content/index';
import Header from './../header/index';

configure({ adapter: new Adapter() });
describe('Content', () => {
    describe('render', () => {
        test('sidebar is expanded', () => {
            // Act
            const component = renderer.create(
                <Content isSidebarExpanded>Test</Content>
            );

            // Assert
            let tree = component.toJSON();
            expect(tree).toMatchSnapshot();
        });

        it('sidebar is not expanded', () => {
            // Arrange
            const component = renderer.create(
                <Content isSidebarExpanded={false}>Test</Content>
            );

            // Assert
            let tree = component.toJSON();
            expect(tree).toMatchSnapshot();
        });

        it('Render Children', () => {
            let children = <div>Test</div>;
            const component = shallow(
                <Content isSidebarExpanded>
                    {children}
                </Content>
            );

            expect(component.contains(children)).toBeTruthy();
        });

        it('Component include header', () => {
            let children = <div>Test</div>;
            const wrapper = shallow(
                <Content isSidebarExpanded>
                    {children}
                </Content>
            );

            expect(wrapper.find(Header)).toHaveLength(1);
        });
    });
})