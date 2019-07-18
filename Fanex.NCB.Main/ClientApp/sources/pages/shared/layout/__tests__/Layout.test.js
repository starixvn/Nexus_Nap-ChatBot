import React from 'react';
import { Layout } from './../layout';
import { shallow, configure } from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';
import ShallowRenderer from 'react-test-renderer/shallow';
import renderer from 'react-test-renderer';

configure({ adapter: new Adapter() })

describe('Layout', () => {
    describe('render', () => {
        it('sidebar is expanded as default', () => {
            // Arrange
            const props = {}

            // Act
            const renderOutput = (new ShallowRenderer()).render(<Layout {...props} />)

            // Assert
            expect(renderOutput).toMatchSnapshot()
        })
    })
})
