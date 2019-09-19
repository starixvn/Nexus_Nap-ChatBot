import React from 'react';
import { Content } from './../content/index';
import { configure, shallow } from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';
import ShallowRenderer from 'react-test-renderer/shallow';
import renderer from 'react-test-renderer';

configure({ adapter: new Adapter() });

describe('Content', () => {
    describe('render', () => {
        it('sidebar is expanded', () => {
            // Act
            const component = renderer.create(<Content isSidebarExpanded />).toJSON();

            // Assert
            expect(component).toMatchSnapshot();
        });

        it('sidebar is not expanded', () => {
            // Arrange
            const props = {
                isSidebarExpanded: false
            };

            // Act
            const renderOutput = (new ShallowRenderer()).render(<Content {...props} />);

            // Assert
            expect(renderOutput).toMatchSnapshot();
        });
    });
})